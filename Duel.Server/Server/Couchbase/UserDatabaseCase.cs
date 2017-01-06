using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x2;
using Couchbase;
using Events.Database;

namespace Server.Couchbase
{
    /// <summary>
    /// Process Couchbase User queries. 
    /// Runs in MultiThreadFlow. All functions need to be thread-safe.
    /// </summary>
    public class UserDatabaseCase : Case
    {
        // Suppose ClusterHelper is configured correctly.
        string bucketName;

        public UserDatabaseCase(string bucketName)
        {
            this.bucketName = bucketName;
        }

        protected override void Setup()
        {
            base.Setup();

            new EventCreateOrLoadUserReq().Bind(OnCreateOrLoad);
            new EventLoadUserReq().Bind(OnLoad);
            new EventUpdateUserReq().Bind(OnUpdate);
        }

        void OnCreateOrLoad(EventCreateOrLoadUserReq req)
        {
            var bucket = ClusterHelper.GetBucket(bucketName );

            var result = bucket.GetDocument<Shared.User>(req.Account);

            if (result.Success)
            {
                var user = result.Content;

                PostLoadResp(req.Context, user, Events.ErrorCodes.Success);
            }
            else
            {
                var user = new Shared.User
                {
                    Account = req.Account,
                    Password = req.Password,
                    Nick = req.Nick,
                    DeviceId = req.DeviceId,
                    Gold = 10000000,
                };

                var document = new Document<Shared.User>
                {
                    Id = req.Account,
                    Content = user
                };

                result = bucket.Insert(document);

                if ( result.Success)
                {
                    PostLoadResp(req.Context, user, Events.ErrorCodes.Success);
                }
                else
                {
                    PostLoadResp(req.Context, user, Events.ErrorCodes.FailDb_Op);
                }
            }
        }

        void OnLoad(EventLoadUserReq req)
        {
            var bucket = ClusterHelper.GetBucket(bucketName);

            var result = bucket.GetDocument<Shared.User>(req.Account);

            if (result.Success)
            {
                var user = result.Content;

                PostLoadResp(req.Context, user, Events.ErrorCodes.Success);
            }
            else
            {
                var user = new Shared.User
                {
                    Account = req.Account,
                };

                PostLoadResp(req.Context, user, Events.ErrorCodes.FailDb_Op);
            }
        }

        void OnUpdate(EventUpdateUserReq req)
        {
            var bucket = ClusterHelper.GetBucket(bucketName);

            var user = new Shared.User
            {
                Account = req.Account,
                Password = req.Password,
                Nick = req.Nick,
                DeviceId = req.DeviceId,
                Gold = req.Gold,
            };

            var document = new Document<Shared.User>
            {
                Id = req.Account,
                Content = user
            };

            var result = bucket.Upsert(document);

            if (result.Success)
            {
                PostUpdateResp(req.Context, user, Events.ErrorCodes.Success);
            }
            else
            {
                PostUpdateResp(req.Context, user, Events.ErrorCodes.FailDb_Op);
            }
        }

        void PostLoadResp(int context, Shared.User user, Events.ErrorCodes result)
        {
            new EventLoadUserResp
            {
                Account = user.Account,
                Context = context,
                Result = (int)result,
                Nick = user.Nick,
                Password = user.Password,
                DeviceId = user.DeviceId,
                Gold = user.Gold
            }
            .Post();
        }

        void PostUpdateResp(int context, Shared.User user, Events.ErrorCodes result)
        {
            new EventUpdateUserResp
            {
                Account = user.Account,
                Context = context,
                Result = (int)result,
                Nick = user.Nick,
                Password = user.Password,
                DeviceId = user.DeviceId,
                Gold = user.Gold
            }
           .Post();
        }
    }
}
