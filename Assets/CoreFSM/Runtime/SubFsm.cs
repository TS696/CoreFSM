using System;
using System.Collections.Generic;

namespace CoreFSM
{
    public abstract class SubFsm<TFsm, TSubFsm> : IState<TFsm>, IFsm<TSubFsm>
        where TFsm : IFsm<TFsm>
        where TSubFsm : IFsm<TSubFsm>
    {
        private readonly StateRunner<TSubFsm> _stateRunner;

        public IState<TSubFsm> CurrentState => _stateRunner.CurrentState;
        public bool IsEnded => _stateRunner.IsEnded;

        protected SubFsm(IEnumerable<IState<TSubFsm>> states, Type startStateType)
        {
            _stateRunner = new StateRunner<TSubFsm>(states, startStateType);
        }

        protected virtual NextState<TFsm> OnEndTransition() => NextState<TFsm>.Continue();
        protected virtual NextState<TFsm> OnPostTickTransition() => NextState<TFsm>.Continue();

        void IState<TFsm>.OnEnter()
        {
            if (_stateRunner.IsEnded)
            {
                _stateRunner.Reset();
            }

            _stateRunner.Tick();
        }

        NextState<TFsm> IState<TFsm>.OnTick()
        {
            _stateRunner.Tick();
            if (_stateRunner.IsEnded)
            {
                return OnEndTransition();
            }

            return OnPostTickTransition();
        }

        void IState<TFsm>.OnExit()
        {
            _stateRunner.Stop();
        }

        void IState<TFsm>.OnDestroy()
        {
            _stateRunner.Dispose();
        }
    }
}
