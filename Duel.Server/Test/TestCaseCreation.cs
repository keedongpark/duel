using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using x2;

namespace Test
{
    [TestFixture]
    class TestCaseCreation
    {
        [TearDown]
        public void TearDown()
        {
            Hub.Shutdown();
            Hub.Instance.DetachAll();
        }

        /// <summary>
        /// Test creation of a Case outside the holder Flow
        /// - The creator needs to know the owning flow
        /// - Flow.Add, Case.Setup need to be called manually
        /// </summary>
        [Test]
        public void TestDynamicCreation()
        {
            var f1 = new SingleThreadFlow();
            var f2 = new SingleThreadFlow();

            Hub.Instance
                .Attach(f1)
                .Attach(f2);

            // Case.Setup has a holder argument.

            Hub.Startup();

            var c1 = new TestCase();

            f1.Add(c1);
            c1.Setup(f1);

            Assert.IsTrue(Object.ReferenceEquals(f1, c1.Flow));

        }

        [Test]
        public void TestCaseSuicide()
        {
            var f1 = new SingleThreadFlow();
            var f2 = new SingleThreadFlow();

            Hub.Instance
                .Attach(f1)
                .Attach(f2);

            // Case.Setup has a holder argument.

            Hub.Startup();

            var c1 = new TestCase();

            f1.Add(c1);
            c1.Setup(f1);

            Assert.IsTrue(Object.ReferenceEquals(f1, c1.Flow));

            c1.Suicide();

            Thread.Sleep(100);

        }

        class TestCase : Case
        {

            public void Suicide()
            {
                Flow.Remove(this);
                Teardown(Flow);
            }

            protected override void Setup()
            {
                base.Setup();


            }
        }
    }
}
