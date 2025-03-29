using System;
using System.Collections.Generic;

namespace CoreFSM
{
    public abstract class SubFsm<TFsm, TInner> : IFsm<TInner>, IState<TFsm>
        where TFsm : IFsm<TFsm>
        where TInner : IFsm<TInner>
    {
        private readonly StateRunner<TInner> _stateRunner;

        public IState<TInner> CurrentState => _stateRunner.CurrentState;
        public bool IsEnded => _stateRunner.IsEnded;

        protected SubFsm(IEnumerable<IState<TInner>> states, Type startStateType)
        {
            _stateRunner = new StateRunner<TInner>(states, startStateType);
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
