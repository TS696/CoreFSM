using System;
using System.Collections.Generic;

namespace CoreFSM
{
    public class StateRunner<T> : IDisposable where T : IFsm<T>
    {
        private readonly IFsm<T> _fsm;

        public IState<T> CurrentState { get; private set; }

        public bool IsEnded => CurrentState == EndState<T>.Instance || _isDisposed;

        private readonly Dictionary<Type, IState<T>> _states = new();

        private bool _isDisposed;

        public StateRunner(IFsm<T> fsm)
        {
            _fsm = fsm;
            CurrentState = EntryState<T>.Instance;
            foreach (var state in fsm.States)
            {
                _states.Add(state.StateType, state);
            }
        }

        private IState<T> FindState(Type type)
        {
            if (type == typeof(EndState<T>))
            {
                return EndState<T>.Instance;
            }

            if (_states.TryGetValue(type, out var state))
            {
                return state;
            }

            throw new InvalidOperationException($"State of type {type} not found in FSM.");
        }

        public void Tick()
        {
            ThrowIfDisposed();

            if (CurrentState is EntryState<T>)
            {
                CurrentState = FindState(_fsm.StartStateType);
                CurrentState.OnEnter();
            }

            var nextState = CurrentState.OnTick();

            if (nextState.IsTransition)
            {
                var newState = FindState(nextState.NextStateType);

                CurrentState.OnExit();
                CurrentState = newState;
                CurrentState?.OnEnter();
            }
        }

        public void Stop()
        {
            ThrowIfDisposed();

            CurrentState.OnExit();
            CurrentState = EndState<T>.Instance;
        }

        public void Reset()
        {
            ThrowIfDisposed();
            if (CurrentState is not EndState<T>)
            {
                throw new InvalidOperationException("Cannot reset while not in EndState.");
            }

            CurrentState = EntryState<T>.Instance;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(StateRunner<T>));
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            foreach (var state in _states.Values)
            {
                state.OnDestroy();
            }
        }
    }
}
