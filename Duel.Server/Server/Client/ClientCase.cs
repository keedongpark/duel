using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x2;

namespace Server.Client
{
    /// <summary>
    /// ClientCase to build server.
    /// </summary>
    public class ClientCase : Case
    {
        string account;
        string password;

        int serverId;
        int instanceId;
        List<Events.Instance.Member> members;

        public bool HasLogin
        {
            get; private set;
        }

        public bool IsMatched
        {
            get; private set;
        }

        public List<Events.Instance.Member> Members
        {
            get { return members; }
        }

        public ClientCase(string account, string password)
        {
            this.account = account;
            this.password = password;
        }

        public void RequestLogin()
        {
            new Events.Login.EventLoginReq
            {
                Account = account,
                Password = password
            }.Post();
        }

        public void RequestLogout()
        {
            new Events.Login.EventLogout
            {
                Account = account
            }.Post();
        }

        public void RequestMatch()
        {
            new Events.Instance.EventMatchReq
            {
                Zone = 1, 
                Requester = new Events.Instance.Member
                {
                    Account = account, 
                    Nick = "", 
                    Gold = 100000,
                    Bot = false
                }
            }
            .Post();
        }


        protected override void Setup()
        {
            base.Setup();

            new Events.Login.EventLoginResp()
                .Bind(OnLoginResp);

            new Events.Instance.EventMatchResp()
                .Bind(OnMatchResp);
        }

        void OnLoginResp(Events.Login.EventLoginResp resp)
        {
            if ( resp.Result == (int)Events.ErrorCodes.Success)
            {
                HasLogin = true;
            }
            else
            {
                HasLogin = false;
            }
        }

        void OnMatchResp(Events.Instance.EventMatchResp resp)
        {
            if ( resp.Result == (int)Events.ErrorCodes.Success)
            {
                IsMatched = true;

                serverId = resp.ServerId;
                instanceId = 0;
                members = resp.Members;
            }
            else
            {
                IsMatched = false;
            }
        }
    }
}
