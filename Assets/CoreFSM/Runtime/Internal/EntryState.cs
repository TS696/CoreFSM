namespace CoreFSM
{
    internal class EntryState<TFsm> : StateBase<TFsm> where TFsm : IFsm<TFsm>
    {
        public static EntryState<TFsm> Instance { get; } = new();

        public override NextState<TFsm> OnTick() => Continue();
    }
}
