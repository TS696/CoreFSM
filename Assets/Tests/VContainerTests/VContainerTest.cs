using CoreFSM;
using CoreFSM.VContainer;
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
                builder.RegisterFsm<TestFsm>(fsmBuilder =>
                {
                    fsmBuilder.RegisterStartState<TestEntryState<TestState>>();
                    fsmBuilder.RegisterState<TestState>();
                });
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
                builder.RegisterFsm<TestFsm>(fsmBuilder =>
                {
                    fsmBuilder.RegisterStartState<TestEntryState<TestSubFsm>>();
                    fsmBuilder.RegisterSubFsm<TestSubFsm>(subFsmBuilder =>
                    {
                        subFsmBuilder.RegisterStartState<TestSubState>();
                    });
                });
            };

            _testLifetimeScope.Build();

            var container = _testLifetimeScope.Container;

            var testFsm = container.Resolve<TestFsm>();
            testFsm.Tick();
            Assert.That(testFsm.CurrentState, Is.InstanceOf<TestSubFsm>());
            testFsm.Dispose();
        }
    }

    public class TestFsm : Fsm<TestFsm>
    {
        public TestFsm(IEnumerable<IState<TestFsm>> states, Type startStateType) : base(states, startStateType)
        {
        }
    }

    public class TestEntryState<TState> : IState<TestFsm> where TState : IState<TestFsm>
    {
        public NextState<TestFsm> OnTick()
        {
            return NextState<TestFsm>.TransitionTo<TState>();
        }
    }

    public class TestState : IState<TestFsm>
    {
    }

    public class TestSubFsm : SubFsm<TestFsm, TestSubFsm>
    {
        public TestSubFsm(IEnumerable<IState<TestSubFsm>> states, Type startStateType) : base(states, startStateType)
        {
        }
    }

    public class TestSubState : IState<TestSubFsm>
    {
    }
}
