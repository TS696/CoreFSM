namespace CoreFSM
{
    internal class EndState<T> : IState<T> where T : IFsm<T>
    {
        public static EndState<T> Instance { get; } = new();
    }
}
