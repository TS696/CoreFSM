using System;
using System.Collections.Generic;

namespace CoreFSM
{
    public abstract class SubFsm<TFsm, TSubFsm> : StateBase<TFsm>, IFsm<TSubFsm>
        where TFsm : IFsm<TFsm>
        where TSubFsm : IFsm<TSubFsm>
    {
        private readonly StateRunner<TSubFsm> _stateRunner;

        public StateBase<TSubFsm> CurrentState => _stateRunner.CurrentState;
        public bool IsEnded => _stateRunner.IsEnded;

        protected SubFsm(IEnumerable<StateBase<TSubFsm>> states, Type startStateType)
        {
            _stateRunner = new StateRunner<TSubFsm>(states, startStateType);
        }

        protected virtual NextState<TFsm> OnEndTransition() => Continue();
        protected virtual NextState<TFsm> OnPostTickTransition() => Continue();

        public override void OnEnter()
        {
            if (_stateRunner.IsEnded)
            {
                _stateRunner.Reset();
            }

            _stateRunner.Tick();
        }

        public override NextState<TFsm> OnTick()
        {
            _stateRunner.Tick();
            if (_stateRunner.IsEnded)
            {
                return OnEndTransition();
            }

            return OnPostTickTransition();
        }

        public override void OnExit()
        {
            _stateRunner.Stop();
        }

        public override void OnDestroy()
        {
            _stateRunner.Dispose();
        }
    }
}
