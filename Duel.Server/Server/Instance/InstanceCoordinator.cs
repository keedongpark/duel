using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x2;
using Events.Instance;

namespace Server.Instance
{
    /// <summary>
    /// Creates an instance on join request with id 0.
    /// </summary>
    public class InstanceCoordinator : Case
    {
        int serverId;
        List<Flow> flows;
        RangedIntPool idPool;
        Random rand;
        int instanceCount;

        public InstanceCoordinator(int serverId, List<Flow> flows)
        {
            this.serverId = serverId;
            this.flows = new List<Flow>();
            this.flows.AddRange(flows);

            this.idPool = new RangedIntPool(1, Int32.MaxValue, true);
            this.rand = new Random();
        }

        protected override void Setup()
        {
            base.Setup();

            // Process Join with InstanceId == 0 only
            new EventJoinReq
            {
                InstanceId = 0
            }
            .Bind(OnJoinReq);

            new EventStatus().Bind(OnStatus);

            PostState();
        }

        void OnJoinReq(EventJoinReq req)
        {
            if (flows == null || flows.Count == 0)
            {
                throw new InvalidOperationException(
                    "Flows should not be empty!"
                );
            }

            var flow = flows[rand.Next(flows.Count)];
            var id = idPool.Acquire();

            var ic = new Instance(id);
            flow.Add(ic);
            ic.Setup(flow);

            req.InstanceId = id;
            req.Post();

            instanceCount++;

            PostState();
        }

        void OnStatus(EventStatus ntf)
        {
            if ( ntf.Status == (int)Events.InstanceStatus.Destroyed)
            {
                idPool.Release(ntf.InstanceId);

                instanceCount--;

                PostState();
            }
        }

        void PostState()
        {
            new EventCoordStatus
            {
                ServerId = serverId, 
                Count = instanceCount
            }
            .Post();
        }
    }
}
