namespace CoreFSM
{
    public abstract class SubFsmBase<TFsm> : IState<TFsm> where TFsm : IFsm<TFsm>
    {
        internal abstract IStateRunner StateRunner { get; }

        protected virtual NextState<TFsm> OnEndTransition() => NextState<TFsm>.Continue();
        protected virtual NextState<TFsm> OnPostTickTransition() => NextState<TFsm>.Continue();

        void IState<TFsm>.OnEnter()
        {
            if (StateRunner.IsEnded)
            {
                StateRunner.Reset();
            }

            StateRunner.Tick();
        }

        NextState<TFsm> IState<TFsm>.OnTick()
        {
            StateRunner.Tick();
            if (StateRunner.IsEnded)
            {
                return OnEndTransition();
            }

            return OnPostTickTransition();
        }

        void IState<TFsm>.OnExit()
        {
            StateRunner.Stop();
        }

        void IState<TFsm>.OnDestroy()
        {
            StateRunner.Dispose();
        }
    }
}
