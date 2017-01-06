using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x2;
using Events.Instance;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    class TestMatching
    {
        [TearDown]
        public void TearDown()
        {
            Hub.Shutdown();
            Hub.Instance.DetachAll();
        }

        [Test]
        public void TestMatchingFlow()
        {

        }
    }
}
