using System;
using System.Collections.Generic;

namespace CoreFSM
{
    public abstract class Fsm<TFsm> : IFsm<TFsm> where TFsm : Fsm<TFsm>
    {
        private readonly StateRunner<TFsm> _stateRunner;

        public IState<TFsm> CurrentState => _stateRunner.CurrentState;
        public bool IsEnded => _stateRunner.IsEnded;

        protected Fsm(IEnumerable<IState<TFsm>> states, Type startStateType)
        {
            _stateRunner = new StateRunner<TFsm>(states, startStateType);
        }

        public void Tick()
        {
            _stateRunner.Tick();
        }

        public void Stop()
        {
            _stateRunner.Stop();
        }

        public void Reset()
        {
            _stateRunner.Reset();
        }

        public void Dispose()
        {
            _stateRunner?.Dispose();
        }

        public string DumpCurrentInfo()
        {
            var sb = new System.Text.StringBuilder();
            ((IFsm)this).Dump(sb);
            return sb.ToString();
        }
    }
}
