using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using x2;
using NUnit.Framework;
using Server.Instance;
using Events.Instance;

namespace Test
{
    [TestFixture]
    class TestInstances
    {
        [TearDown]
        public void TearDown()
        {
            Hub.Shutdown();
            Hub.Instance.DetachAll();
        }

        [Test]
        public void TestAll()
        {
            var flow = new SingleThreadFlow();

            var flows = new List<Flow>();
            flows.Add(flow);

            var coord = new InstanceCoordinator(1, flows);
            var tc = new TestCase();

            flow.Add(coord);
            flow.Add(tc);

            Hub.Instance.Attach(flow);

            Hub.Startup();

            tc.RequestJoin();

            while ( !tc.Joined )
            {
                Thread.Sleep(10);
            }

            tc.RequestLeave();

            while ( tc.Left != 2)
            {
                Thread.Sleep(10);
            }
        }

        class TestCase : Case
        {
            int instanceId;

            public bool Joined { get; private set; }

            public int Left { get; private set; }

            public void RequestJoin()
            {
                var members = new List<Member>
                {
                    new Member
                    {
                        Account = "A"
                    },
                    new Member
                    {
                        Account = "B"
                    }
                };

                new EventJoinReq
                {
                    ServerId = 0,
                    InstanceId = 0,
                    Members = members
                }
                .Post();

            }

            public void RequestLeave()
            {
                new EventLeaveReq
                {
                    InstanceId = instanceId,
                    Account = "A"
                }
                .Post();

                new EventLeaveReq
                {
                    InstanceId = instanceId,
                    Account = "B"
                }
                .Post();
            }

            protected override void Setup()
            {
                base.Setup();

                new EventJoinResp().Bind(OnJoinResp);
                new EventLeaveResp().Bind(OnLeaveResp);
            }

            void OnJoinResp(EventJoinResp resp)
            {
                instanceId = resp.InstanceId;
                Joined = true;
            }

            void OnLeaveResp(EventLeaveResp resp)
            {
                Left++;
            }

        }
    }
}
