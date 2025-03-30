namespace CoreFSM
{
    public abstract class FsmSlot<TFsm, TSubFsm> : SubFsmBase<TFsm>, IFsm<TSubFsm>
        where TFsm : IFsm<TFsm>
        where TSubFsm : Fsm<TSubFsm>
    {
        private readonly TSubFsm _fsm;
        internal sealed override IStateRunner StateRunner => _fsm.StateRunner;

        public IState<TSubFsm> CurrentState => _fsm.CurrentState;
        public bool IsEnded => _fsm.IsEnded;

        protected FsmSlot(TSubFsm fsm)
        {
            _fsm = fsm;
        }
    }
}
