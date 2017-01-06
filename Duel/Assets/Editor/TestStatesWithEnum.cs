using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Assets.scripts;

namespace Assets.Editor
{
    public enum States : byte
    {
        Alive,
        Idle, 
        Attack
    }

    public static class StatesExt
    {
        public static byte Key(this States v) { return (byte)v; }
    }

    class StateAlive : State<byte>
    {
        public StateAlive()
            : base(States.Alive.Key())
        {
        }
    }

    class StateIdle : State<byte>
    {
        public StateIdle()
            : base(States.Idle.Key())
        {
        }
    }

    class StateAttack : State<byte>
    {
        public StateAttack()
            : base(States.Attack.Key())
        {
        }
    }


    [TestFixture]
    public class TestStatesWithEnum
    {
        [Test]
        public void TestSetup()
        {
            var root = new StateAlive()
                .Add(new StateIdle())
                .Add(new StateAttack());


            new StateMachine<byte>().Initialize(root);

            Assert.IsTrue(root.Has(States.Idle.Key()));
            Assert.IsTrue(root.Has(0));
            Assert.IsTrue(root.Has(1));

            // 되기는 할 텐데 byte 변환을 계속 해야 하는 것이 좀 그렇다. 
            // Key() 확장 함수가 문법 상으로는 더 괜찮아 보인다. 
        }
    }
}
