using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Ninject;
using EcsR3.Infrastructure;
using EcsR3.Infrastructure.Extensions;
using EcsR3.Plugins.Batching;
using EcsR3.Plugins.Persistence;
using EcsR3.Plugins.Views;

namespace EcsR3.Examples.Application
{
    public abstract class EcsR3ConsoleApplication : EcsR3Application
    {
        public override IDependencyRegistry DependencyRegistry { get; } = new NinjectDependencyRegistry();

        protected override void LoadPlugins()
        {
            RegisterPlugin(new ViewsPlugin());
            RegisterPlugin(new BatchPlugin());
            RegisterPlugin(new PersistencePlugin());
        }

        protected override void StartSystems()
        {
            this.StartAllBoundReactiveSystems();
        }
    }
}