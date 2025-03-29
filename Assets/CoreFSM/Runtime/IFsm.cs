namespace CoreFSM
{
    public interface IFsm<T> where T : IFsm<T>
    {
        public IState<T> CurrentState { get; }
        public bool IsEnded { get; }
    }
}
