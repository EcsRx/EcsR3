using SystemsR3.Executor.Handlers;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Pools;
using EcsR3.Collections;
using EcsR3.Collections.Database;
using EcsR3.Collections.Entity;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Groups.Observable;
using EcsR3.Groups.Observable.Tracking;
using EcsR3.Systems.Handlers;

namespace EcsR3.Infrastructure.Modules
{
    public class EcsR3InfrastructureModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            // Register ECS specific infrastructure
            registry.Bind<IIdPool, IdPool>();
            registry.Bind<IEntityFactory, DefaultEntityFactory>();
            registry.Bind<IEntityCollectionFactory, DefaultEntityCollectionFactory>();
            registry.Bind<IEntityDatabase, EntityDatabase>();
            registry.Bind<IObservableGroupFactory, DefaultObservableObservableGroupFactory>();
            registry.Bind<IObservableGroupManager, ObservableGroupManager>();
            registry.Bind<IComponentTypeAssigner, DefaultComponentTypeAssigner>();
            registry.Bind<IComponentTypeLookup>(new BindingConfiguration{ToMethod = CreateDefaultTypeLookup});           
            registry.Bind<IComponentDatabase, ComponentDatabase>();
            registry.Bind<IGroupTrackerFactory, GroupTrackerFactory>();
            
            // Register ECS specific system handlers
            registry.Bind<IConventionalSystemHandler, BasicEntitySystemHandler>();
            registry.Bind<IConventionalSystemHandler, ReactToEntitySystemHandler>();
            registry.Bind<IConventionalSystemHandler, ReactToGroupSystemHandler>();
            registry.Bind<IConventionalSystemHandler, ReactToDataSystemHandler>();
            registry.Bind<IConventionalSystemHandler, SetupSystemHandler>();
            registry.Bind<IConventionalSystemHandler, TeardownSystemHandler>();
        }

        private static object CreateDefaultTypeLookup(IDependencyResolver resolver)
        {
            var componentTypeAssigner = resolver.Resolve<IComponentTypeAssigner>();
            var allComponents = componentTypeAssigner.GenerateComponentLookups();
            return new ComponentTypeLookup(allComponents);
        }
    }
}