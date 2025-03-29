using System;
using System.Collections.Generic;

namespace CoreFSM
{
    public abstract class Fsm<T> : IFsm<T> where T : Fsm<T>
    {
        public abstract Type StartStateType { get; }
        public IEnumerable<IState<T>> States { get; }

        protected Fsm(IEnumerable<IState<T>> states)
        {
            States = states;
        }
    }
}
