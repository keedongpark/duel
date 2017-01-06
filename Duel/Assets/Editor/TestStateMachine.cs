using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Assets.scripts;

[TestFixture]
public class TestStateMachine
{
    class StateAlive : State<string>
    {
        public StateAlive()
            : base("state_alive")
        {
        }
    }

    class StatePeace : State<string>
    {
        public StatePeace()
            : base("state_peace")
        {
        }
    }

    class StateEngage : State<string>
    {
        public StateEngage()
            : base("state_engage")
        {
        }
    }

    class StateIdle : State<string>
    {
        public StateIdle()
            : base("state_idle")
        {
        }
    }

    class StateChase : State<string>
    {
        public StateChase()
            : base("state_chase")
        {
        }
    }

    class StateAttack : State<string>
    {
        public StateAttack()
            : base("state_attack")
        {
        }
    }

    State<string> alive;

    [SetUp]
    public void Setup()
    {
        var machine = new StateMachine<string>();

        alive = new StateAlive()
            .Add(new StatePeace()
                  .Add(new StateIdle()))
            .Add(new StateEngage()
                  .Add(new StateChase())
                  .Add(new StateAttack())
            );

        machine.Initialize(alive)
               .Execute();
    }

    [Test]
    public void TestSetup()
    {
        Assert.IsTrue(alive.Has("state_alive"));

        Assert.IsTrue(alive.Has("state_peace"));
        Assert.IsTrue(alive.Has("state_idle"));

        Assert.IsTrue(alive.Has("state_engage"));
        Assert.IsTrue(alive.Has("state_chase"));
        Assert.IsTrue(alive.Has("state_attack"));

        Assert.IsTrue(alive.Find("state_peace").IsActive == false);
        Assert.IsTrue(alive.Find("state_peace").ActiveChild == null);
        Assert.IsTrue(alive.Find("state_peace").Has("state_idle"));

        Assert.IsTrue(alive.Find("state_idle").Parent == alive.Find("state_peace"));
        Assert.IsTrue(alive.Find("state_idle").IsActive == false);
        Assert.IsTrue(alive.Find("state_idle").ActiveChild == null);

        Assert.IsTrue(alive.Find("state_engage").IsActive == false);
        Assert.IsTrue(alive.Find("state_engage").ActiveChild == null);
        Assert.IsTrue(alive.Find("state_engage").Has("state_chase"));
        Assert.IsTrue(alive.Find("state_engage").Has("state_attack"));

        Assert.IsTrue(alive.Find("state_chase").Parent == alive.Find("state_engage"));
        Assert.IsTrue(alive.Find("state_chase").IsActive == false);
        Assert.IsTrue(alive.Find("state_chase").ActiveChild == null);

        Assert.IsTrue(alive.Find("state_attack").Parent == alive.Find("state_engage"));
        Assert.IsTrue(alive.Find("state_attack").IsActive == false);
        Assert.IsTrue(alive.Find("state_attack").ActiveChild == null);
    }

    [Test]
    public void TestTransitions()
    {
        Assert.IsTrue(alive.Machine.Current == alive);
        Assert.IsTrue(alive.Machine.Root == alive);

        // normal
        alive.Transit("state_idle").Execute();

        var peace = alive.Find("state_peace");

        Assert.IsTrue(peace != null);
        Assert.IsTrue(alive.Machine.Current.Key == "state_idle");
        Assert.IsTrue(peace.ActiveChild.Key == "state_idle");
        Assert.IsTrue(peace.IsActive);

        // tran to parent
        alive.Transit("state_peace").Execute();

        Assert.IsTrue(peace.ActiveChild == null);
        Assert.IsTrue(peace.IsActive);
        Assert.IsTrue(peace.Machine.Current.Key == "state_peace");


        // tran to other branch
        alive.Transit("state_chase").Execute();

        Assert.IsTrue(alive.Machine.Current.Key == "state_chase");
        Assert.IsTrue(alive.Machine.Current.IsActive);
        Assert.IsTrue(alive.Machine.Current.ActiveChild == null);
        Assert.IsTrue(alive.Machine.Current.Parent.Key == "state_engage");

        // tran to sibling
        alive.Transit("state_attack").Execute();

        var attack = alive.Machine.Current;

        Assert.IsTrue(alive.Machine.Current.Key == "state_attack");
        Assert.IsTrue(alive.Machine.Current.IsActive);
        Assert.IsTrue(alive.Machine.Current.ActiveChild == null);
        Assert.IsTrue(alive.Machine.Current.Parent.Key == "state_engage");

        // tran to parent
        alive.Transit("state_engage").Execute();

        Assert.IsTrue(alive.Machine.Current.Key == "state_engage");
        Assert.IsTrue(alive.Machine.Current.IsActive);
        Assert.IsTrue(alive.Machine.Current.ActiveChild == null);
        Assert.IsTrue(alive.Machine.Current.Parent.Key == "state_alive");

        Assert.IsTrue(attack.IsActive == false);
        Assert.IsTrue(attack.ActiveChild == null);
    }

    [Test]
    public void TestStats()
    {
        alive.Transit("state_idle").Execute();
        alive.Transit("state_attack").Execute();

        Assert.IsTrue(alive.Find("state_idle").Stat.enterCount == 1);
        Assert.IsTrue(alive.Find("state_idle").Stat.executionCount == 1);
        Assert.IsTrue(alive.Find("state_idle").Stat.exitCount == 1);

        Assert.IsTrue(alive.Find("state_attack").Stat.enterCount == 1);
        Assert.IsTrue(alive.Find("state_attack").Stat.executionCount == 1);
        Assert.IsTrue(alive.Find("state_attack").Stat.exitCount == 0);

        alive.Transit("state_idle").Execute();

        Assert.IsTrue(alive.Find("state_idle").Stat.enterCount == 2);
        Assert.IsTrue(alive.Find("state_idle").Stat.executionCount == 2);
        Assert.IsTrue(alive.Find("state_idle").Stat.exitCount == 1);


        alive.Transit("state_attack").Execute();

        Assert.IsTrue(alive.Find("state_idle").Stat.executionCount == 2);
        Assert.IsTrue(alive.Find("state_idle").Stat.exitCount == 2);

        Assert.IsTrue(alive.Find("state_attack").Stat.enterCount == 2);
        Assert.IsTrue(alive.Find("state_attack").Stat.executionCount == 2);
        Assert.IsTrue(alive.Find("state_attack").Stat.exitCount == 1);
    }
} 
