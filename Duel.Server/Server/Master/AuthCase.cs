using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x2;
using Events;
using Events.Login;
using Events.Database;

namespace Server.Master
{
    /// <summary>
    /// Authenticate and manages User location
    /// </summary>
    public class AuthCase : Case
    {
        class Entry
        {
            public enum State
            {
                LoginPending,       // Pending db request
                Login,              // Login success 
            };

            public State state;
            public Shared.User user;
        }

        Dictionary<string, Entry> dic;

        public AuthCase()
        {
            dic = new Dictionary<string, Entry>();
        }

        protected override void Setup()
        {
            base.Setup();

            new EventMasterLoginReq().Bind(OnLoginReq);
            new EventMasterLogout().Bind(OnLogout);
            new EventLoadUserResp
            {
                Context = (int)EventDatabaseContext.Auth
            }
            .Bind(OnUserLoaded);
        }

        void OnLoginReq(EventMasterLoginReq req)
        {
            Entry entry;

            if ( dic.TryGetValue(req.Account, out entry) )
            {
                // TODO: Kickout 

                dic.Remove(req.Account);
            }

            dic[req.Account] = new Entry
            {
                state = Entry.State.LoginPending,
                user = new Shared.User
                {
                    Account = req.Account,
                    Password = req.Password
                }
            };

            new EventCreateOrLoadUserReq
            {
                Account = req.Account,
                Password = req.Password,
                Context = (int)EventDatabaseContext.Auth
            }.Post();
        }

        void OnUserLoaded(EventLoadUserResp resp)
        {
            Entry entry;

            if (!dic.TryGetValue(resp.Account, out entry))
            {
                Log.Emit(
                   LogLevel.Error,
                   string.Format(
                       "Login> [Account: {0}][Error: User not found from dic]",
                       resp.Account
                   )
                );
                return;
            }

            var user = new Shared.User
            {
                Account = resp.Account,
                Password = resp.Password,
                Nick = resp.Nick,
                DeviceId = resp.DeviceId,
                Gold = resp.Gold,
            };

            int result = resp.Result; 

            if (resp.Result == 0)
            {
                if (user.Password == entry.user.Password)
                {
                    dic[user.Account].state = Entry.State.Login;
                }
                else
                {
                    result = (int)ErrorCodes.FailLogin_PasswordIncorrect;
                }
            }

            if ( result != 0)
            {
                dic.Remove(user.Account);

                Log.Emit(
                    LogLevel.Error, 
                    string.Format(
                        "Login> [Account: {0}][Error: {1}]", 
                        user.Account, result
                    )
                );
            }
            else
            {
                Log.Emit(
                    LogLevel.Info, 
                    string.Format(
                        "Login> [Account: {0}]",
                        user.Account
                    )
                );
            }

            new EventMasterLoginResp
            {
                Account = user.Account,
                Result = result
            }
            .Post();
        } 

        void OnLogout(EventMasterLogout req)
        {
            dic.Remove(req.Account);

            Log.Emit(
                  LogLevel.Info,
                  string.Format(
                      "Logout> [Account: {0}]",
                      req.Account
                  )
            );
        }
    }
}
