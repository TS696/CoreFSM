using System;

namespace CoreFSM
{
    public readonly struct NextState<T> where T : IFsm<T>
    {
        internal readonly bool IsTransition;
        internal readonly Type NextStateType;

        private NextState(bool isTransition, Type nextStateType)
        {
            IsTransition = isTransition;
            NextStateType = nextStateType;
        }

        public static NextState<T> TransitionTo<TNext>() where TNext : IState<T>
        {
            return new NextState<T>(true, typeof(TNext));
        }

        public static NextState<T> Continue()
        {
            return new NextState<T>(false, null);
        }

        public static NextState<T> End()
        {
            return TransitionTo<EndState<T>>();
        }
    }
}
