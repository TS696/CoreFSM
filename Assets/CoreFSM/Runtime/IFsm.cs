using System;
using System.Collections.Generic;

namespace CoreFSM
{
    public interface IFsm<T> where T : IFsm<T>
    {
        Type StartStateType { get; }
        IEnumerable<IState<T>> States { get; }
    }
}
