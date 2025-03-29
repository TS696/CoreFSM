namespace CoreFSM
{
    public abstract class FsmSlot<TFsm, TInner> : IFsm<TInner>, IState<TFsm>
        where TFsm : IFsm<TFsm>
        where TInner : Fsm<TInner>
    {
        private readonly TInner _fsm;
        public IState<TInner> CurrentState => _fsm.CurrentState;
        public bool IsEnded => _fsm.IsEnded;

        protected FsmSlot(TInner fsm)
        {
            _fsm = fsm;
        }

        protected virtual NextState<TFsm> OnEndTransition() => NextState<TFsm>.Continue();
        protected virtual NextState<TFsm> OnPostTickTransition() => NextState<TFsm>.Continue();

        void IState<TFsm>.OnEnter()
        {
            _fsm.Tick();
        }

        NextState<TFsm> IState<TFsm>.OnTick()
        {
            _fsm.Tick();
            if (_fsm.IsEnded)
            {
                return OnEndTransition();
            }

            return OnPostTickTransition();
        }

        void IState<TFsm>.OnExit()
        {
            _fsm.Stop();
        }

        void IState<TFsm>.OnDestroy()
        {
            _fsm.Dispose();
        }
    }
}
