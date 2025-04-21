#if COREFSM_USE_VCONTAINER
using System;
using VContainer;

namespace CoreFSM.VContainer
{
    public class FsmBuilder<TFsm> where TFsm : IFsm<TFsm>
    {
        private readonly IContainerBuilder _containerBuilder;
        private Type _startStateType;
        internal Type StartStateType => _startStateType;

        public FsmBuilder(IContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        public void RegisterStartState<TState>() where TState : StateBase<TFsm>
        {
            _startStateType = typeof(TState);
            RegisterState<TState>();
        }

        public void RegisterState<TState>() where TState : StateBase<TFsm>
        {
            _containerBuilder.Register<StateBase<TFsm>, TState>(Lifetime.Singleton);
        }

        public void RegisterSubFsm<TSubFsm>(Action<FsmBuilder<TSubFsm>> configure)
            where TSubFsm : SubFsm<TFsm, TSubFsm>
        {
            var subFsmBuilder = new FsmBuilder<TSubFsm>(_containerBuilder);
            configure(subFsmBuilder);
            subFsmBuilder.OnConfigured();
            _containerBuilder.Register<StateBase<TFsm>, TSubFsm>(Lifetime.Singleton)
                .WithParameter(subFsmBuilder.StartStateType);
        }

        internal void OnConfigured()
        {
            if (_startStateType == null)
            {
                throw new InvalidOperationException($"Start state type is not registered. Call {nameof(RegisterStartState)}().");
            }
        }
    }

    public static class VContainerExtensions
    {
        public static void RegisterFsm<TFsm>(this IContainerBuilder containerBuilder, Action<FsmBuilder<TFsm>> configure)
            where TFsm : IFsm<TFsm>
        {
            var fsmBuilder = new FsmBuilder<TFsm>(containerBuilder);
            configure(fsmBuilder);
            fsmBuilder.OnConfigured();
            containerBuilder.Register<TFsm>(Lifetime.Singleton).WithParameter(fsmBuilder.StartStateType);
        }
    }
}
#endif
