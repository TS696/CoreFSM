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
            var states = new List<StateBase<SingleStateFsm>>
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
            var states = new List<StateBase<SingleStateFsm>>
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
            var states = new List<StateBase<MultiStateFsm>>
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
            var states = new List<StateBase<MultiStateFsm>>
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
            public SingleStateFsm(IEnumerable<StateBase<SingleStateFsm>> states) : base(states, typeof(SingleState))
            {
            }
        }

        private class SingleState : StateBase<SingleStateFsm>
        {
            public bool IsDestroyed { get; private set; }

            public override NextState<SingleStateFsm> OnTick()
            {
                return End();
            }

            public override void OnDestroy()
            {
                IsDestroyed = true;
            }
        }

        private class MultiStateFsm : Fsm<MultiStateFsm>
        {
            public MultiStateFsm(IEnumerable<StateBase<MultiStateFsm>> states) : base(states, typeof(MultiStateFsmEntryPoint))
            {
            }
        }

        private class MultiStateFsmEntryPoint : StateBase<MultiStateFsm>
        {
            public override NextState<MultiStateFsm> OnTick()
            {
                return To<MultiStateFsmCore>();
            }
        }

        private class MultiStateFsmCore : StateBase<MultiStateFsm>
        {
            public bool IsDestroyed { get; private set; }

            public override NextState<MultiStateFsm> OnTick()
            {
                return To<MultiStateFsmExitPoint>();
            }

            public override void OnDestroy()
            {
                IsDestroyed = true;
            }
        }

        private class MultiStateFsmExitPoint : StateBase<MultiStateFsm>
        {
            public override NextState<MultiStateFsm> OnTick()
            {
                return End();
            }
        }
    }
}
