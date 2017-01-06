using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x2;

namespace Server.Session
{
    public class UserCase : Case
    {
        Shared.User user;

        public UserCase(Shared.User user)
        {
            this.user = user;
        }

        protected override void Setup()
        {
            base.Setup();

            // Match Request 

            // Match Response 
        }
    }
}
