using CoreFSM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Tests.VContainerTests
{
    public class VContainerTest
    {
        private TestLifetimeScope _testLifetimeScope;

        [SetUp]
        public void SetUp()
        {
            var testObject = new GameObject();
            _testLifetimeScope = testObject.AddComponent<TestLifetimeScope>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_testLifetimeScope != null)
            {
                _testLifetimeScope.Dispose();
                UnityEngine.Object.Destroy(_testLifetimeScope.gameObject);
            }
        }

        [Test]
        public void FsmInjectTest()
        {
            _testLifetimeScope.OnConfigure = builder =>
            {
                builder.Register<TestFsm>(Lifetime.Singleton);
                builder.Register<IState<TestFsm>, TestEntryState<TestState>>(Lifetime.Singleton);
                builder.Register<IState<TestFsm>, TestState>(Lifetime.Singleton);
            };

            _testLifetimeScope.Build();

            var container = _testLifetimeScope.Container;

            var testFsm = container.Resolve<TestFsm>();
            testFsm.Tick();
            Assert.That(testFsm.CurrentState, Is.InstanceOf<TestState>());
            testFsm.Dispose();
        }

        [Test]
        public void SubFsmInjectTest()
        {
            _testLifetimeScope.OnConfigure = builder =>
            {
                builder.Register<TestFsm>(Lifetime.Singleton);
                builder.Register<IState<TestFsm>, TestEntryState<TestSubFsm>>(Lifetime.Singleton);
                builder.Register<IState<TestFsm>, TestSubFsm>(Lifetime.Singleton);
                builder.Register<IState<TestSubFsm>, TestSubState>(Lifetime.Singleton);
            };

            _testLifetimeScope.Build();

            var container = _testLifetimeScope.Container;

            var testFsm = container.Resolve<TestFsm>();
            testFsm.Tick();
            Assert.That(testFsm.CurrentState, Is.InstanceOf<TestSubFsm>());
            testFsm.Dispose();
        }

        [Test]
        public void FsmSlotInjectTest()
        {
            _testLifetimeScope.OnConfigure = builder =>
            {
                builder.Register<TestFsm>(Lifetime.Singleton);
                builder.Register<IState<TestFsm>, TestEntryState<TestFsmSlot>>(Lifetime.Singleton);
                builder.Register<IState<TestFsm>, TestFsmSlot>(Lifetime.Singleton);
                builder.Register<TestChildFsm>(Lifetime.Singleton);
                builder.Register<IState<TestChildFsm>, TestChildState>(Lifetime.Singleton);
            };

            _testLifetimeScope.Build();

            var container = _testLifetimeScope.Container;

            var testFsm = container.Resolve<TestFsm>();
            testFsm.Tick();
            Assert.That(testFsm.CurrentState, Is.InstanceOf<TestFsmSlot>());
            testFsm.Dispose();
        }
    }

    public class TestFsm : Fsm<TestFsm>
    {
        public TestFsm(IEnumerable<IState<TestFsm>> states) : base(states, typeof(TestEntryState<>))
        {
        }
    }

    public class TestEntryState<T> : IState<TestFsm> where T : IState<TestFsm>
    {
        public Type StateType => typeof(TestEntryState<>);

        public NextState<TestFsm> OnTick()
        {
            return NextState<TestFsm>.TransitionTo<T>();
        }
    }

    public class TestState : IState<TestFsm>
    {
    }

    public class TestSubFsm : SubFsm<TestFsm, TestSubFsm>
    {
        public TestSubFsm(IEnumerable<IState<TestSubFsm>> states) : base(states, typeof(TestSubState))
        {
        }
    }

    public class TestSubState : IState<TestSubFsm>
    {
    }

    public class TestFsmSlot : FsmSlot<TestFsm, TestChildFsm>
    {
        public TestFsmSlot(TestChildFsm fsm) : base(fsm)
        {
        }
    }

    public class TestChildFsm : Fsm<TestChildFsm>
    {
        public TestChildFsm(IEnumerable<IState<TestChildFsm>> states) : base(states, typeof(TestChildState))
        {
        }
    }

    public class TestChildState : IState<TestChildFsm>
    {
    }
}
