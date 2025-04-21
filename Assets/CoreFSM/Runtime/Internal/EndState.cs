namespace CoreFSM
{
    internal class EndState<TFsm> : StateBase<TFsm> where TFsm : IFsm<TFsm>
    {
        public static EndState<TFsm> Instance { get; } = new();

        public override NextState<TFsm> OnTick() => Continue();
    }
}
