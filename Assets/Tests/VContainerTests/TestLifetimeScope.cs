using System;
using VContainer;
using VContainer.Unity;

namespace Tests.VContainerTests
{
    public class TestLifetimeScope : LifetimeScope
    {
        public Action<IContainerBuilder> OnConfigure { get; set; }

        protected override void Configure(IContainerBuilder builder)
        {
            OnConfigure?.Invoke(builder);
        }
    }
}
