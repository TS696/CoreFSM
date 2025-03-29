namespace CoreFSM
{
    public abstract class FsmSlot<TFsm, TInner> : SubFsmBase<TFsm>, IFsm<TInner>
        where TFsm : IFsm<TFsm>
        where TInner : Fsm<TInner>
    {
        private readonly TInner _fsm;
        internal sealed override IStateRunner StateRunner => _fsm.StateRunner;

        public IState<TInner> CurrentState => _fsm.CurrentState;
        public bool IsEnded => _fsm.IsEnded;

        protected FsmSlot(TInner fsm)
        {
            _fsm = fsm;
        }
    }
}
