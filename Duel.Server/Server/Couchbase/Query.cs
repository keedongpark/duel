using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Couchbase
{
    public abstract class Query
    {
        public enum Result
        {
            Success, 
            Fail
        }

        public string  Reason { get; private set; }

        public abstract Result Execute();

        public abstract void Complete();
    }
}
