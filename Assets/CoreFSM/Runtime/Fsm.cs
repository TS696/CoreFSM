using System;
using System.Collections.Generic;

namespace CoreFSM
{
    public abstract class Fsm<T> : IFsm<T> where T : Fsm<T>
    {
        private readonly StateRunner<T> _stateRunner;
        internal StateRunner<T> StateRunner => _stateRunner;

        public IState<T> CurrentState => _stateRunner.CurrentState;
        public bool IsEnded => _stateRunner.IsEnded;

        protected Fsm(IEnumerable<IState<T>> states, Type startStateType)
        {
            _stateRunner = new StateRunner<T>(states, startStateType);
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
