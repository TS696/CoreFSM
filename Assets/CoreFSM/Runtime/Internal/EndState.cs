namespace CoreFSM
{
    internal class EndState<TFsm> : IState<TFsm> where TFsm : IFsm<TFsm>
    {
        public static EndState<TFsm> Instance { get; } = new();
    }
}
