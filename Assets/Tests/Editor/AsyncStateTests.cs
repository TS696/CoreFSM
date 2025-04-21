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
            var states = new List<StateBase<AsyncStateFsm>>
            {
                new AsyncTickTestState(),
            };

            var fsm = new AsyncStateFsm(states, typeof(AsyncTickTestState));
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

            var states = new List<StateBase<AsyncStateFsm>>
            {
                asyncCancelTestState,
            };

            var fsm = new AsyncStateFsm(states, typeof(AsyncCancelTestState));
            fsm.Tick();
            fsm.Dispose();

            yield return null;

            Assert.That(asyncCancelTestState.IsCancelled, Is.True);
        }

        private class AsyncStateFsm : Fsm<AsyncStateFsm>
        {
            public AsyncStateFsm(IEnumerable<StateBase<AsyncStateFsm>> states, Type startStateType) : base(states, startStateType)
            {
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
                return End();
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

                return End();
            }
        }
    }
}
