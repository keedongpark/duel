using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using x2;
using NUnit.Framework;
using Events.Timer;

namespace Test
{
    [TestFixture]
    class TestTimers
    {
        /// <summary>
        /// 하나의 이벤트로 여러 타이머의 구분이 가능한 지 알아본다. 
        /// 타잎으로 구분하고 클래스 내부적으로 필요한 구분에 활용한다. 
        /// </summary>
        [Test]
        public void TestTimerIdDispatch()
        {
            Hub.Instance.Attach(TimeFlow.Default);

            var tc = new TestCase();

            Hub.Instance.Attach(new SingleThreadFlow().Add(tc));

            Hub.Startup();

            while ( tc.TwoCount < 2)
            {
                Thread.Sleep(10);
            }

            Assert.IsTrue(tc.OneCount > 10);

            Hub.Shutdown();

            Hub.Instance.DetachAll();
        }

        class TestCase : Case
        {
            public int OneCount
            {
                get; private set;
            }

            public int TwoCount
            {
                get; private set;
            }

            protected override void Setup()
            {
                base.Setup();

                new EventTimerBase
                {
                    Context = 1
                }
                .Bind(OnTimerOne);

                new EventTimerBase
                {
                    Context = 2
                }
                .Bind(OnTimerTwo);

                TimeFlow.Default.ReserveRepetition(new EventTimerBase { Context = 1 }, TimeSpan.FromMilliseconds(10));
                TimeFlow.Default.ReserveRepetition(new EventTimerBase { Context = 2 }, TimeSpan.FromMilliseconds(100));
            }

            void OnTimerOne(EventTimerBase e)
            {
                OneCount++;
            }

            void OnTimerTwo(EventTimerBase e)
            {
                TwoCount++;
            }
        }
    }
}
