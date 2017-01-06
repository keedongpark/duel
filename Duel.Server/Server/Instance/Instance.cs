using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x2;
using Events;
using Events.Instance;

namespace Server.Instance
{
    public class Instance  : Case
    {
        int id;
        List<Member> members;

        public Instance(int id)
        {
            this.id = id;
            members = new List<Member>();
        }

        void OnJoinReq(EventJoinReq req)
        {
            members.AddRange(req.Members);

            new EventJoinResp
            {
                ServerId = req.ServerId,
                InstanceId = id,
                Result = (int)ErrorCodes.Success,
                Members = members
            }
            .InResponseOf(req)
            .Post();
        }

        void OnLeaveReq(EventLeaveReq req)
        {
            var member = members.FirstOrDefault(
                m => m.Account == req.Account
            );

            int result = (int)ErrorCodes.Success;

            if ( member == null )
            {
                result = (int)ErrorCodes.FailInstance_MemberNotFound;
            }
            else
            {
                members.Remove(member);
            }

            new EventLeaveResp
            {
                ServerId = req.ServerId,
                InstanceId = id,
                Account = req.Account, 
                Result = result
            }
            .Post();

            new EventStatus
            {
                ServerId = req.ServerId,
                InstanceId = id,
                Status = (int)InstanceStatus.MemberCount,
                Members = members
            }
            .Post();

            // TODO: 게임 플레이 구현

            if ( members.Count == 0 )
            {
                new EventDestroySelf
                {
                    InstanceId = id
                }
                .Post();

                // TODO: 게임 방 상태를 소멸로 처리. 진입 안 되도록 함.
            }
        }

        void OnDestroySelf(EventDestroySelf e)
        {
            Flow.Remove(this);
            Teardown(Flow);
        }

        protected override void Setup()
        {
            base.Setup();

            new EventJoinReq
            {
                InstanceId = id
            }
            .Bind(OnJoinReq);

            new EventLeaveReq
            {
                InstanceId = id
            }
            .Bind(OnLeaveReq);

            new EventDestroySelf
            {
                InstanceId = id
            }
            .Bind(OnDestroySelf);
        }

        protected override void Teardown()
        {
            base.Teardown();

            new EventStatus
            {
                InstanceId = id,
                Status = (int)InstanceStatus.Destroyed,
            }
           .Post();
        }
    }
}
