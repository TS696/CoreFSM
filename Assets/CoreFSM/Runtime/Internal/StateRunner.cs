using System;
using System.Collections.Generic;

namespace CoreFSM
{
    internal class StateRunner<TFsm> where TFsm : IFsm<TFsm>
    {
        private readonly Type _startStateType;

        public IState<TFsm> CurrentState { get; private set; }

        public bool IsEnded => CurrentState == EndState<TFsm>.Instance || _isDisposed;

        private readonly Dictionary<Type, IState<TFsm>> _states = new();

        private bool _isDisposed;
        private bool _hasStarted;

        public StateRunner(IEnumerable<IState<TFsm>> states, Type startStateType)
        {
            _startStateType = startStateType;
            CurrentState = EntryState<TFsm>.Instance;
            foreach (var state in states)
            {
                _states.Add(state.GetType(), state);
            }
        }

        private IState<TFsm> FindState(Type type)
        {
            if (type == typeof(EndState<TFsm>))
            {
                return EndState<TFsm>.Instance;
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

            if (!_hasStarted)
            {
                _hasStarted = true;
                CurrentState = FindState(_startStateType);
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
            CurrentState = EndState<TFsm>.Instance;
        }

        public void Reset()
        {
            ThrowIfDisposed();
            if (CurrentState is not EndState<TFsm>)
            {
                throw new InvalidOperationException("Cannot reset while not in EndState.");
            }

            CurrentState = EntryState<TFsm>.Instance;
            _hasStarted = false;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(StateRunner<TFsm>));
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
