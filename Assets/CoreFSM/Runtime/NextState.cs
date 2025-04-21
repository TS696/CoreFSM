using System;

namespace CoreFSM
{
    public readonly struct NextState<TFsm> where TFsm : IFsm<TFsm>
    {
        internal readonly bool IsTransition;
        internal readonly Type NextStateType;

        internal NextState(bool isTransition, Type nextStateType)
        {
            IsTransition = isTransition;
            NextStateType = nextStateType;
        }
    }
}
