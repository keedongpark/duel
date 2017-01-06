using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x2;
using Events.Login;

namespace Server.Session
{
    /// <summary>
    /// LoginCase which creates UserCase after Auth finished.
    /// </summary>
    public class LoginCase : Case
    {
        Random rand;
        List<Flow> userCaseHolders;

        public LoginCase(Flow userCaseHolder)
        {
            this.userCaseHolders = new List<Flow>();
            userCaseHolders.Add(userCaseHolder);
        }

        public LoginCase(List<Flow> userCaseHolders)
        {
            this.userCaseHolders = userCaseHolders;
        }

        protected override void Setup()
        {
            base.Setup();

            rand = new Random();

            new EventLoginReq()
                .Bind(OnLoginReq);

            new EventMasterLoginResp()
                .Bind(OnLoginResp);
        }

        void OnLoginReq(EventLoginReq req)
        {
            new EventMasterLoginReq
            {
                Account = req.Account,
                Password = req.Password
            }
            .Post();
        }

        void OnLoginResp(EventMasterLoginResp resp)
        {
            if ( userCaseHolders == null || userCaseHolders.Count == 0)
            {
                throw new InvalidOperationException(
                    "UserCase holders should not be null or empty"
                );
            }

            var index = rand.Next(userCaseHolders.Count);
            var holder = userCaseHolders[index];

            var uc = new UserCase(
                new Shared.User
                {
                    Account = resp.Account
                }
            );

            holder.Add(uc);
            uc.Setup(holder);

            new EventLoginResp
            {
                Account = resp.Account,
                Result = resp.Result
            }
            .Post();
        }
    }
}
