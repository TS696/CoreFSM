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
            var childStates = new List<IState<ChildFsm>>
            {
                new ChildFsmEntryPoint<ChildFsmCore>(),
                new ChildFsmCore(),
                new ChildFsmExitPoint()
            };
            var childFsm = new ChildFsm(childStates);

            var states = new List<IState<ParentFsm>>
            {
                new ParentFsmEntryPoint(),
                childFsm,
                new ParentFsmExitPoint()
            };

            var fsm = new ParentFsm(states);

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
        public void FsmSlotTest()
        {
            var grandChildStates = new List<IState<GrandChildFsm>>
            {
                new GrandChildFsmEntryPoint(),
                new GrandChildFsmExitPoint()
            };
            var grandChildFsm = new GrandChildFsm(grandChildStates);
            var grandChildSlot = new GrandChildFsmSlot(grandChildFsm);

            var childStates = new List<IState<ChildFsm>>
            {
                new ChildFsmEntryPoint<GrandChildFsmSlot>(),
                grandChildSlot,
                new ChildFsmExitPoint()
            };
            var childFsm = new ChildFsm(childStates);

            var states = new List<IState<ParentFsm>>
            {
                new ParentFsmEntryPoint(),
                childFsm,
                new ParentFsmExitPoint()
            };
            var fsm = new ParentFsm(states);

            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<ChildFsm>());
            Assert.That(childFsm.CurrentState, Is.InstanceOf<GrandChildFsmSlot>());
            Assert.That(grandChildSlot.CurrentState, Is.InstanceOf<GrandChildFsmExitPoint>());
            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<ChildFsm>());
            Assert.That(childFsm.CurrentState, Is.InstanceOf<ChildFsmExitPoint>());
            Assert.That(grandChildSlot.IsEnded, Is.True);
            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<ParentFsmExitPoint>());
            Assert.That(childFsm.IsEnded, Is.True);
            fsm.Tick();
            Assert.That(fsm.IsEnded, Is.True);
            fsm.Dispose();
        }


        private class ParentFsm : Fsm<ParentFsm>
        {
            public ParentFsm(IEnumerable<IState<ParentFsm>> states) : base(states, typeof(ParentFsmEntryPoint))
            {
            }
        }

        private class ParentFsmEntryPoint : IState<ParentFsm>
        {
            public NextState<ParentFsm> OnTick()
            {
                return NextState<ParentFsm>.TransitionTo<ChildFsm>();
            }
        }

        private class ParentFsmExitPoint : IState<ParentFsm>
        {
            public NextState<ParentFsm> OnTick()
            {
                return NextState<ParentFsm>.End();
            }
        }

        private class ChildFsm : SubFsm<ParentFsm, ChildFsm>
        {
            public ChildFsm(IEnumerable<IState<ChildFsm>> states) : base(states, typeof(ChildFsmEntryPoint<>))
            {
            }

            protected override NextState<ParentFsm> OnEndTransition()
            {
                return NextState<ParentFsm>.TransitionTo<ParentFsmExitPoint>();
            }
        }

        private class ChildFsmEntryPoint<T> : IState<ChildFsm> where T : IState<ChildFsm>
        {
            public Type StateType => typeof(ChildFsmEntryPoint<>);

            public NextState<ChildFsm> OnTick()
            {
                return NextState<ChildFsm>.TransitionTo<T>();
            }
        }

        private class ChildFsmCore : IState<ChildFsm>
        {
            public NextState<ChildFsm> OnTick()
            {
                return NextState<ChildFsm>.TransitionTo<ChildFsmExitPoint>();
            }
        }

        private class ChildFsmExitPoint : IState<ChildFsm>
        {
            public NextState<ChildFsm> OnTick()
            {
                return NextState<ChildFsm>.End();
            }
        }

        private class GrandChildFsmSlot : FsmSlot<ChildFsm, GrandChildFsm>
        {
            public GrandChildFsmSlot(GrandChildFsm fsm) : base(fsm)
            {
            }

            protected override NextState<ChildFsm> OnEndTransition()
            {
                return NextState<ChildFsm>.TransitionTo<ChildFsmExitPoint>();
            }
        }

        private class GrandChildFsm : Fsm<GrandChildFsm>
        {
            public GrandChildFsm(IEnumerable<IState<GrandChildFsm>> states) : base(states, typeof(GrandChildFsmEntryPoint))
            {
            }
        }

        private class GrandChildFsmEntryPoint : IState<GrandChildFsm>
        {
            public NextState<GrandChildFsm> OnTick()
            {
                return NextState<GrandChildFsm>.TransitionTo<GrandChildFsmExitPoint>();
            }
        }

        private class GrandChildFsmExitPoint : IState<GrandChildFsm>
        {
            public NextState<GrandChildFsm> OnTick()
            {
                return NextState<GrandChildFsm>.End();
            }
        }
    }
}
