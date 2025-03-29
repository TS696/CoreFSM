using System;
using System.Collections.Generic;

namespace CoreFSM
{
    public abstract class SubFsm<TFsm, TInner> : SubFsmBase<TFsm>, IFsm<TInner>
        where TFsm : IFsm<TFsm>
        where TInner : IFsm<TInner>
    {
        private readonly StateRunner<TInner> _stateRunner;
        protected sealed override IStateRunner StateRunner => _stateRunner;

        public IState<TInner> CurrentState => _stateRunner.CurrentState;
        public bool IsEnded => _stateRunner.IsEnded;

        protected SubFsm(IEnumerable<IState<TInner>> states, Type startStateType)
        {
            _stateRunner = new StateRunner<TInner>(states, startStateType);
        }
    }
}
