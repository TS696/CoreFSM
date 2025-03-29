using CoreFSM;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests.Editor
{
    public class FsmTests
    {
        [Test]
        public void SingleStateTickTest()
        {
            var states = new List<IState<SingleStateFsm>>
            {
                new SingleState()
            };

            var fsm = new SingleStateFsm(states);

            fsm.Tick();
            Assert.That(fsm.IsEnded, Is.True);
            fsm.Dispose();
        }

        [Test]
        public void DestroyTest()
        {
            var singleState = new SingleState();
            var states = new List<IState<SingleStateFsm>>
            {
                singleState
            };

            var fsm = new SingleStateFsm(states);
            fsm.Dispose();
            Assert.That(singleState.IsDestroyed, Is.True);

            Assert.Throws(typeof(ObjectDisposedException), () =>
            {
                fsm.Tick();
            });
        }


        [Test]
        public void MultiStateTickTest()
        {
            var states = new List<IState<MultiStateFsm>>
            {
                new MultiStateFsmEntryPoint(),
                new MultiStateFsmCore(),
                new MultiStateFsmExitPoint()
            };

            var fsm = new MultiStateFsm(states);

            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<MultiStateFsmCore>());
            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<MultiStateFsmExitPoint>());
            fsm.Tick();
            Assert.That(fsm.IsEnded, Is.True);
            fsm.Dispose();
        }

        [Test]
        public void ResetTest()
        {
            var states = new List<IState<MultiStateFsm>>
            {
                new MultiStateFsmEntryPoint(),
                new MultiStateFsmCore(),
                new MultiStateFsmExitPoint()
            };

            var fsm = new MultiStateFsm(states);

            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<MultiStateFsmCore>());
            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<MultiStateFsmExitPoint>());

            fsm.Stop();
            fsm.Reset();

            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<MultiStateFsmCore>());
            fsm.Tick();
            Assert.That(fsm.CurrentState, Is.InstanceOf<MultiStateFsmExitPoint>());
            fsm.Tick();
            Assert.That(fsm.IsEnded, Is.True);
            fsm.Dispose();
        }

        private class SingleStateFsm : Fsm<SingleStateFsm>
        {
            public SingleStateFsm(IEnumerable<IState<SingleStateFsm>> states) : base(states, typeof(SingleState))
            {
            }
        }

        private class SingleState : IState<SingleStateFsm>
        {
            public bool IsDestroyed { get; private set; }

            public NextState<SingleStateFsm> OnTick()
            {
                return NextState<SingleStateFsm>.End();
            }

            public void OnDestroy()
            {
                IsDestroyed = true;
            }
        }

        private class MultiStateFsm : Fsm<MultiStateFsm>
        {
            public MultiStateFsm(IEnumerable<IState<MultiStateFsm>> states) : base(states, typeof(MultiStateFsmEntryPoint))
            {
            }
        }

        private class MultiStateFsmEntryPoint : IState<MultiStateFsm>
        {
            public NextState<MultiStateFsm> OnTick()
            {
                return NextState<MultiStateFsm>.TransitionTo<MultiStateFsmCore>();
            }
        }

        private class MultiStateFsmCore : IState<MultiStateFsm>
        {
            public bool IsDestroyed { get; private set; }

            public NextState<MultiStateFsm> OnTick()
            {
                return NextState<MultiStateFsm>.TransitionTo<MultiStateFsmExitPoint>();
            }

            public void OnDestroy()
            {
                IsDestroyed = true;
            }
        }

        private class MultiStateFsmExitPoint : IState<MultiStateFsm>
        {
            public NextState<MultiStateFsm> OnTick()
            {
                return NextState<MultiStateFsm>.End();
            }
        }
    }
}
