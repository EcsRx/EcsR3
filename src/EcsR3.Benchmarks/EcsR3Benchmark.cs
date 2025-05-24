using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EcsR3.Collections;
using EcsR3.Collections.Entity;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Computeds;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities.Routing;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Infrastructure.Modules;
using SystemsR3.Executor;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Infrastructure.Modules;
using SystemsR3.Infrastructure.Ninject;

namespace EcsR3.Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 1)]
    [MemoryDiagnoser]
    [MarkdownExporter]
    public abstract class EcsR3Benchmark
    {
        public IDependencyRegistry DependencyRegistry;
        public IDependencyResolver DependencyResolver;
        public IEntityCollection EntityCollection;
        public IComponentDatabase ComponentDatabase;
        public IComponentTypeLookup ComponentTypeLookup;
        public ISystemExecutor SystemExecutor;
        public IEntityChangeRouter EntityChangeRouter;
        public IComputedEntityGroupRegistry ComputedEntityGroupRegistry;
        
        // This pulls in a lot of types which let the component type lookup know about them
        public RandomGroupFactory RandomGroupFactory { get; } = new RandomGroupFactory();
        
        public virtual ComponentDatabaseConfig OverrideComponentDatabaseConfig() => new() { OnlyPreAllocatePoolsWithConfig = true };
        
        public IComponentPool<T> GetPoolFor<T>() where T : IComponent
        {
            return ComponentDatabase.GetPoolFor<T>(ComponentTypeLookup.GetComponentTypeId(typeof(T)));
        }

        protected EcsR3Benchmark()
        {
            DependencyRegistry = new NinjectDependencyRegistry();
            DependencyRegistry.LoadModule(new FrameworkModule());
            DependencyRegistry.LoadModule(new EcsR3InfrastructureModule());
            DependencyRegistry.Unbind<ComponentDatabaseConfig>();
            DependencyRegistry.Bind<ComponentDatabaseConfig>(x => x.ToInstance(OverrideComponentDatabaseConfig()));
            
            DependencyResolver = DependencyRegistry.BuildResolver();
            
            EntityCollection = DependencyResolver.Resolve<IEntityCollection>();
            ComputedEntityGroupRegistry = DependencyResolver.Resolve<IComputedEntityGroupRegistry>();
            ComponentDatabase = DependencyResolver.Resolve<IComponentDatabase>();
            ComponentTypeLookup = DependencyResolver.Resolve<IComponentTypeLookup>();
            SystemExecutor = DependencyResolver.Resolve<ISystemExecutor>();
            EntityChangeRouter = DependencyResolver.Resolve<EntityChangeRouter>();
        }

        [GlobalSetup]
        public virtual void Setup()
        {}

        [GlobalCleanup]
        public virtual void Cleanup()
        {}
    }
}