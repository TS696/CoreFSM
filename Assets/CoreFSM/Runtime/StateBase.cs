using System.Diagnostics.Contracts;

namespace CoreFSM
{
    public abstract class StateBase<TFsm> where TFsm : IFsm<TFsm>
    {
        [Pure]
        protected static NextState<TFsm> Continue()
        {
            return default;
        }

        [Pure]
        protected static NextState<TFsm> To<TState>() where TState : StateBase<TFsm>
        {
            return new NextState<TFsm>(true, typeof(TState));
        }
        
        [Pure]
        protected static NextState<TFsm> End()
        {
            return To<EndState<TFsm>>();
        }

        public virtual void OnEnter()
        {
        }

        public abstract NextState<TFsm> OnTick();

        public virtual void OnExit()
        {
        }

        public virtual void OnDestroy()
        {
        }
    }
}
