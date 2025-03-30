namespace CoreFSM
{
    public interface IState<TFsm> where TFsm : IFsm<TFsm>
    {
        void OnEnter()
        {
        }

        NextState<TFsm> OnTick()
        {
            return NextState<TFsm>.Continue();
        }

        void OnExit()
        {
        }

        void OnDestroy()
        {
        }
    }
}
