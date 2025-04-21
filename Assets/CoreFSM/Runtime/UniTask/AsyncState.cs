#if COREFSM_USE_UNITASK
using Cysharp.Threading.Tasks;
using System.Threading;

namespace CoreFSM.UniTask
{
    public abstract class AsyncState<TFsm> : StateBase<TFsm>
        where TFsm : IFsm<TFsm>
    {
        private CancellationTokenSource _cancellationTokenSource;
        private NextState<TFsm>? _nextState;
        private ChannelWriter<AsyncUnit> _tickWriter;

        public override void OnEnter()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var channel = Channel.CreateSingleConsumerUnbounded<AsyncUnit>();
            _tickWriter = channel.Writer;
            Cysharp.Threading.Tasks.UniTask.Void(async token =>
            {
                _nextState = await Run(channel.Reader, token);
            }, _cancellationTokenSource.Token);
        }

        protected abstract UniTask<NextState<TFsm>> Run(ChannelReader<AsyncUnit> tickReader, CancellationToken cancellationToken);

        public override NextState<TFsm> OnTick()
        {
            if (_nextState.HasValue)
            {
                return _nextState.Value;
            }

            _tickWriter.TryWrite(AsyncUnit.Default);
            return Continue();
        }

        public override void OnExit()
        {
            _tickWriter.TryComplete();
            CancelTask();
        }

        public override void OnDestroy()
        {
            CancelTask();
        }

        private void CancelTask()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}
#endif
