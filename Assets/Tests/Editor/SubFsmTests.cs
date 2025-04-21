using CoreFSM;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests.Editor
{
    public class SubFsmTests
    {
        [Test]
        public void SubFsmTest()
        {
            var childStates = new List<StateBase<ChildFsm>>
            {
                new ChildFsmEntryPoint<ChildFsmCore>(),
                new ChildFsmCore(),
                new ChildFsmExitPoint()
            };
            var childFsm = new ChildFsm(childStates, typeof(ChildFsmEntryPoint<ChildFsmCore>));

            var states = new List<StateBase<ParentFsm>>
            {
                new ParentFsmEntryPoint<ChildFsm>(),
                childFsm,
                new ParentFsmExitPoint()
            };

            var fsm = new ParentFsm(states, typeof(ParentFsmEntryPoint<ChildFsm>));

            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<ChildFsm>());
            Assert.That(childFsm.CurrentState, Is.InstanceOf<ChildFsmCore>());
            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<ChildFsm>());
            Assert.That(childFsm.CurrentState, Is.InstanceOf<ChildFsmExitPoint>());
            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<ParentFsmExitPoint>());
            Assert.That(childFsm.IsEnded, Is.True);
            fsm.Tick();
            Assert.That(fsm.IsEnded, Is.True);
            fsm.Dispose();
        }

        [Test]
        public void SubFsmReEntryTest()
        {
            var childStates = new List<StateBase<ChildFsm>>
            {
                new ChildFsmEntryPoint<ChildFsmCore>(),
                new ChildFsmCore(),
                new ChildFsmExitPoint()
            };
            var childFsm = new ChildFsmReEntry(childStates, typeof(ChildFsmEntryPoint<ChildFsmCore>));

            var states = new List<StateBase<ParentFsm>>
            {
                new ParentFsmEntryPoint<ChildFsmReEntry>(),
                childFsm,
                new ParentFsmExitPoint()
            };

            var fsm = new ParentFsm(states, typeof(ParentFsmEntryPoint<ChildFsmReEntry>));

            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<ChildFsmReEntry>());
            Assert.That(childFsm.CurrentState, Is.InstanceOf<ChildFsmCore>());
            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<ChildFsmReEntry>());
            Assert.That(childFsm.CurrentState, Is.InstanceOf<ChildFsmCore>());
            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<ChildFsmReEntry>());
            Assert.That(childFsm.CurrentState, Is.InstanceOf<ChildFsmExitPoint>());
            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<ParentFsmExitPoint>());
            Assert.That(childFsm.IsEnded, Is.True);
            fsm.Tick();
            Assert.That(fsm.IsEnded, Is.True);
            fsm.Dispose();
        }


        private class ParentFsm : Fsm<ParentFsm>
        {
            public ParentFsm(IEnumerable<StateBase<ParentFsm>> states, Type startStateType) : base(states, startStateType)
            {
            }
        }

        private class ParentFsmEntryPoint<TState> : StateBase<ParentFsm> where TState : StateBase<ParentFsm>
        {
            public override NextState<ParentFsm> OnTick()
            {
                return To<TState>();
            }
        }

        private class ParentFsmExitPoint : StateBase<ParentFsm>
        {
            public override NextState<ParentFsm> OnTick()
            {
                return End();
            }
        }

        private class ChildFsm : SubFsm<ParentFsm, ChildFsm>
        {
            public ChildFsm(IEnumerable<StateBase<ChildFsm>> states, Type startStateType) : base(states, startStateType)
            {
            }

            protected override NextState<ParentFsm> OnEndTransition()
            {
                return To<ParentFsmExitPoint>();
            }
        }

        private class ChildFsmReEntry : SubFsm<ParentFsm, ChildFsm>
        {
            private int _tickCount;

            public ChildFsmReEntry(IEnumerable<StateBase<ChildFsm>> states, Type startStateType) : base(states, startStateType)
            {
            }

            protected override NextState<ParentFsm> OnEndTransition()
            {
                return To<ParentFsmExitPoint>();
            }

            protected override NextState<ParentFsm> OnPostTickTransition()
            {
                if (_tickCount > 0)
                {
                    return Continue();
                }

                _tickCount++;
                return To<ChildFsmReEntry>();
            }
        }

        private class ChildFsmEntryPoint<TState> : StateBase<ChildFsm> where TState : StateBase<ChildFsm>
        {
            public override NextState<ChildFsm> OnTick()
            {
                return To<TState>();
            }
        }

        private class ChildFsmCore : StateBase<ChildFsm>
        {
            public override NextState<ChildFsm> OnTick()
            {
                return To<ChildFsmExitPoint>();
            }
        }

        private class ChildFsmExitPoint : StateBase<ChildFsm>
        {
            public override NextState<ChildFsm> OnTick()
            {
                return End();
            }
        }
    }
}
