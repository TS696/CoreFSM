namespace CoreFSM
{
    internal class EntryState<T> : IState<T> where T : IFsm<T>
    {
        public static EntryState<T> Instance { get; } = new();
    }
}
