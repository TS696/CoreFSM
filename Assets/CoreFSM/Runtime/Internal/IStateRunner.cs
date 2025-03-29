using System;

namespace CoreFSM
{
    internal interface IStateRunner : IDisposable
    {
        bool IsEnded { get; }
        void Tick();
        void Stop();
        void Reset();
    }
}
