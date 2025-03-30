using System;

namespace CoreFSM
{
    public readonly struct NextState<TFsm> where TFsm : IFsm<TFsm>
    {
        internal readonly bool IsTransition;
        internal readonly Type NextStateType;

        private NextState(bool isTransition, Type nextStateType)
        {
            IsTransition = isTransition;
            NextStateType = nextStateType;
        }

        public static NextState<TFsm> TransitionTo<TNext>() where TNext : IState<TFsm>
        {
            return new NextState<TFsm>(true, typeof(TNext));
        }

        public static NextState<TFsm> Continue()
        {
            return new NextState<TFsm>(false, null);
        }

        public static NextState<TFsm> End()
        {
            return TransitionTo<EndState<TFsm>>();
        }
    }
}
