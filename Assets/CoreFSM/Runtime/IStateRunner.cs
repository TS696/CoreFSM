using System;

namespace CoreFSM
{
    public interface IStateRunner : IDisposable
    {
        bool IsEnded { get; }
        void Tick();
        void Stop();
        void Reset();
    }
}
