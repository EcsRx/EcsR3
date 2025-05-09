using EcsR3.Components.Database;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Ninject;
using EcsR3.Infrastructure;
using EcsR3.Infrastructure.Extensions;
using EcsR3.Plugins.Batching;
using EcsR3.Plugins.Persistence;
using EcsR3.Plugins.Views;
using SystemsR3.Infrastructure.Extensions;

namespace EcsR3.Examples.Application
{
    public abstract class EcsR3ConsoleApplication : EcsR3Application
    {
        public override IDependencyRegistry DependencyRegistry { get; } = new NinjectDependencyRegistry();

        public virtual ComponentDatabaseConfig GetComponentDatabaseConfig { get; } = new();
        
        protected override void LoadPlugins()
        {
            RegisterPlugin(new ViewsPlugin());
            RegisterPlugin(new BatchPlugin());
            RegisterPlugin(new PersistencePlugin());
            
            DependencyRegistry.Unbind<ComponentDatabaseConfig>();
            DependencyRegistry.Bind<ComponentDatabaseConfig>(x => x.ToInstance(GetComponentDatabaseConfig));
        }

        protected override void StartSystems()
        {
            this.StartAllBoundReactiveSystems();
        }
    }
}