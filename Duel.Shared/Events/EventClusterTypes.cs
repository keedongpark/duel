﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events
{
    public enum EventClusterTypes
    {
        ServerList = EventTypeRange.ClusterBegin, 
        Join,
        Leave,
        End,
    }
}
