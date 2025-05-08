using System.Linq;
using SystemsR3.Executor.Handlers;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Pools;
using EcsR3.Collections;
using EcsR3.Collections.Entity;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Entities.Routing;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Groups.Observable;
using EcsR3.Groups.Observable.Tracking;
using EcsR3.Systems.Handlers;

namespace EcsR3.Examples.ExampleApps.Performance.Modules
{
    public class OptimizedEcsRxInfrastructureModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            registry.Bind<IIdPool, IdPool>();
            registry.Bind<IEntityFactory, DefaultEntityFactory>();
            registry.Bind<IEntityCollection, EntityCollection>();
            registry.Bind<IObservableGroupFactory, ObservableGroupFactory>();
            registry.Bind<IObservableGroupManager, ObservableGroupManager>();
            registry.Bind<IConventionalSystemHandler, BasicEntitySystemHandler>();
            registry.Bind<IComponentTypeAssigner, DefaultComponentTypeAssigner>();
            registry.Bind<IGroupTrackerFactory, GroupTrackerFactory>();
            registry.Bind<IEntityChangeRouter, EntityChangeRouter>();
            
            var componentNamespace = typeof(ClassComponent1).Namespace;
            var componentTypes = typeof(ClassComponent1).Assembly.GetTypes().Where(x => x.Namespace == componentNamespace);
            var explicitTypeLookups = componentTypes.Select((type, i) => new { type, i }).ToDictionary(x => x.type, x => x.i);
            var explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups);

            registry.Bind<IComponentTypeLookup>(new BindingConfiguration{ToInstance = explicitComponentLookup});           
            registry.Bind<IComponentDatabase, ComponentDatabase>();
        }
    }
}