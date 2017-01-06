using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Timer
{
    enum EventTimerTypes
    {
        TimerBegin = EventTypeRange.TimerBegin,
        Timer,
        TimerMatch,
        TimerCoordinator, 
    }
}
