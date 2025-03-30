#if COREFSM_USE_UNITASK
using CoreFSM;
using CoreFSM.UniTask;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor
{
    public class AsyncStateTests
    {
        [Test]
        public void AsyncStateTickTest()
        {
            var states = new List<IState<AsyncStateFsm>>
            {
                new AsyncStateEntryPoint<AsyncTickTestState>(),
                new AsyncTickTestState(),
            };

            var fsm = new AsyncStateFsm(states);
            fsm.Tick();
            fsm.Tick();
            LogAssert.Expect("tick 1");
            fsm.Tick();
            LogAssert.Expect("tick 2");
            fsm.Tick();
            LogAssert.Expect("tick 3");
            fsm.Dispose();
        }
        
        [UnityTest]
        public IEnumerator AsyncStateCancelTest()
        {
            var asyncCancelTestState = new AsyncCancelTestState();

            var states = new List<IState<AsyncStateFsm>>
            {
                new AsyncStateEntryPoint<AsyncCancelTestState>(),
                asyncCancelTestState,
            };

            var fsm = new AsyncStateFsm(states);
            fsm.Tick();
            fsm.Tick();
            fsm.Dispose();

            yield return null;

            Assert.That(asyncCancelTestState.IsCancelled, Is.True);
        }

        private class AsyncStateFsm : Fsm<AsyncStateFsm>
        {
            public AsyncStateFsm(IEnumerable<IState<AsyncStateFsm>> states) : base(states, typeof(AsyncStateEntryPoint<>))
            {
            }
        }

        private class AsyncStateEntryPoint<TState> : IState<AsyncStateFsm> where TState : IState<AsyncStateFsm>
        {
            public Type StateType => typeof(AsyncStateEntryPoint<>);

            NextState<AsyncStateFsm> IState<AsyncStateFsm>.OnTick()
            {
                return NextState<AsyncStateFsm>.TransitionTo<TState>();
            }
        }

        private class AsyncTickTestState : AsyncState<AsyncStateFsm>
        {
            protected override async UniTask<NextState<AsyncStateFsm>> Run(ChannelReader<AsyncUnit> tickReader, CancellationToken cancellationToken)
            {
                Debug.Log("tick 1");
                await tickReader.WaitToReadAsync(cancellationToken);
                Debug.Log("tick 2");
                await tickReader.WaitToReadAsync(cancellationToken);
                Debug.Log("tick 3");
                return NextState<AsyncStateFsm>.End();
            }
        }

        private class AsyncCancelTestState : AsyncState<AsyncStateFsm>
        {
            public bool IsCancelled { get; private set; }

            protected override async UniTask<NextState<AsyncStateFsm>> Run(ChannelReader<AsyncUnit> tickReader, CancellationToken cancellationToken)
            {
                try
                {
                    await UniTask.Delay(1, cancellationToken: cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    IsCancelled = true;
                }

                return NextState<AsyncStateFsm>.End();
            }
        }
    }
}
#endif
