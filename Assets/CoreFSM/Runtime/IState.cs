namespace CoreFSM
{
    public interface IState<T> where T : IFsm<T>
    {
        void OnEnter()
        {
        }

        NextState<T> OnTick()
        {
            return NextState<T>.Continue();
        }

        void OnExit()
        {
        }

        void OnDestroy()
        {
        }
    }
}
