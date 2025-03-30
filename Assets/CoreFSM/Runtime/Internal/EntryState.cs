namespace CoreFSM
{
    internal class EntryState<TFsm> : IState<TFsm> where TFsm : IFsm<TFsm>
    {
        public static EntryState<TFsm> Instance { get; } = new();
    }
}
