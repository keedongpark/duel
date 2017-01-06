using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Instance
{
    /// <summary>
    /// 4001 ~ 5000
    /// </summary>
    public enum EventInstanceTypes
    {
        Base = EventTypeRange.InstanceBegin, 
        MatchReq, 
        MatchResp, 
        CreateReq, 
        CreateResp, 
        JoinReq, 
        JoinResp, 
        LeaveReq, 
        LeaveResp,
        Status, 
        DestroySelf, 
        CoordStatus
    }
}
