using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Infrastructure;
using EcsR3.Plugins.Persistence;
using EcsR3.Plugins.Views;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Infrastructure.Ninject;

namespace EcsR3.Examples.Application;

public abstract class EcsR3BenchmarkConsoleApplication : EcsR3Application
{
    public override IDependencyRegistry DependencyRegistry { get; } = new NinjectDependencyRegistry();

    public virtual ComponentDatabaseConfig GetComponentDatabaseConfig { get; } = new();
    
    public IComponentTypeLookup ComponentTypeLookup { get; private set; }

    protected virtual IEnumerable<Type> SpecifyComponentsToIncludeInPool()
    {
        var componentNamespace = typeof(ClassComponent1).Namespace;
        return typeof(ClassComponent1).Assembly.GetTypes().Where(x => x.Namespace == componentNamespace);
    }
    
    protected override void LoadPlugins()
    {
        RegisterPlugin(new ViewsPlugin());
        RegisterPlugin(new PersistencePlugin());
            
        DependencyRegistry.Unbind<ComponentDatabaseConfig>();
        DependencyRegistry.Bind<ComponentDatabaseConfig>(x => x.ToInstance(GetComponentDatabaseConfig));
        
        var componentTypes = SpecifyComponentsToIncludeInPool();
        var explicitTypeLookups = componentTypes.Select((type, i) => new { type, i }).ToDictionary(x => x.type, x => x.i);
        var explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups);

        DependencyRegistry.Unbind<IComponentTypeLookup>();
        DependencyRegistry.Bind<IComponentTypeLookup>(new BindingConfiguration{ToInstance = explicitComponentLookup});
    }

    protected override void ResolveApplicationDependencies()
    {
        base.ResolveApplicationDependencies();
        ComponentTypeLookup = DependencyResolver.Resolve<IComponentTypeLookup>();
    }

    public IComponentPool<T> GetPoolFor<T>() where T : IComponent
    { return ComponentDatabase.GetPoolFor<T>(ComponentTypeLookup.GetComponentTypeId(typeof(T))); }

    protected override void StartSystems()
    {}
}