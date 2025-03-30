using System;
using System.Collections.Generic;

namespace CoreFSM
{
    public abstract class SubFsm<TFsm, TSubFsm> : SubFsmBase<TFsm>, IFsm<TSubFsm>
        where TFsm : IFsm<TFsm>
        where TSubFsm : IFsm<TSubFsm>
    {
        private readonly StateRunner<TSubFsm> _stateRunner;
        internal sealed override IStateRunner StateRunner => _stateRunner;

        public IState<TSubFsm> CurrentState => _stateRunner.CurrentState;
        public bool IsEnded => _stateRunner.IsEnded;

        protected SubFsm(IEnumerable<IState<TSubFsm>> states, Type startStateType)
        {
            _stateRunner = new StateRunner<TSubFsm>(states, startStateType);
        }
    }
}
