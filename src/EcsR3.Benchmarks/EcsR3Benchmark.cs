using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EcsR3.Collections;
using EcsR3.Collections.Entity;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
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
        public IDependencyRegistry DependencyRegistry { get; }
        public IDependencyResolver DependencyResolver { get; }
        public IEntityCollection EntityCollection { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public ISystemExecutor SystemExecutor { get; }
        public IEntityChangeRouter EntityChangeRouter { get; }
        public IObservableGroupManager ObservableGroupManager { get; }
        
        // This pulls in a lot of types which let the component type lookup know about them
        public RandomGroupFactory RandomGroupFactory { get; } = new RandomGroupFactory();
        
        public virtual ComponentDatabaseConfig OverrideComponentDatabaseConfig() => new() { OnlyPreAllocatePoolsWithConfig = true };

        protected EcsR3Benchmark()
        {
            DependencyRegistry = new NinjectDependencyRegistry();
            DependencyRegistry.LoadModule(new FrameworkModule());
            DependencyRegistry.LoadModule(new EcsR3InfrastructureModule());
            DependencyRegistry.Unbind<ComponentDatabaseConfig>();
            DependencyRegistry.Bind<ComponentDatabaseConfig>(x => x.ToInstance(OverrideComponentDatabaseConfig()));
            
            DependencyResolver = DependencyRegistry.BuildResolver();
            
            EntityCollection = DependencyResolver.Resolve<IEntityCollection>();
            ObservableGroupManager = DependencyResolver.Resolve<IObservableGroupManager>();
            ComponentDatabase = DependencyResolver.Resolve<IComponentDatabase>();
            ComponentTypeLookup = DependencyResolver.Resolve<IComponentTypeLookup>();
            SystemExecutor = DependencyResolver.Resolve<ISystemExecutor>();
            EntityChangeRouter = DependencyResolver.Resolve<EntityChangeRouter>();
            
            Console.WriteLine($"Loaded {(ComponentDatabase as ComponentDatabase).ComponentData.Length} Component Pools");
            for (var i = 0; i < ComponentTypeLookup.AllComponentTypeIds.Length; i++)
            {
                Console.WriteLine($"|- Component Type Id: {ComponentTypeLookup.GetComponentType(i).Name}");
            }
        }
        
        public IComponentPool<T> GetPoolFor<T>() where T : IComponent
        {
            return ComponentDatabase.GetPoolFor<T>(ComponentTypeLookup.GetComponentTypeId(typeof(T)));
        }
        
        [GlobalSetup]
        public abstract void Setup();

        [GlobalCleanup]
        public abstract void Cleanup();
    }
}