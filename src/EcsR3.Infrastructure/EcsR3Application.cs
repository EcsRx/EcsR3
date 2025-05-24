using SystemsR3.Infrastructure;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Collections;
using EcsR3.Collections.Entity;
using EcsR3.Components.Database;
using EcsR3.Computeds;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Infrastructure.Modules;

namespace EcsR3.Infrastructure
{
    public abstract class EcsR3Application : SystemsR3Application, IEcsR3Application
    {
        public IEntityCollection EntityCollection { get; private set; }
        public IComponentDatabase ComponentDatabase { get; private set; }
        public IComputedEntityGroupRegistry ComputedEntityGroupRegistry { get; private set; }
        public IComputedComponentGroupRegistry ComputedComponentGroupRegistry { get; private set; }
        
        /// <summary>
        /// Load any modules that your application needs
        /// </summary>
        /// <remarks>
        /// If you wish to use the default setup call through to base, if you wish to stop the default framework
        /// modules loading then do not call base and register your own internal framework module.
        /// </remarks>
        protected override void LoadModules()
        {
            base.LoadModules();
            DependencyRegistry.LoadModule(new EcsR3InfrastructureModule()
            { ComponentDatabaseConfig = OverrideComponentDatabaseConfig() });
        }
        
        /// <summary>
        /// Allows you to override the default component pool database settings, which can hugely reduce allocations
        /// and startup speed, it is entirely optional and defaults are used if nothing is overidden.
        /// </summary>
        /// <returns>A component database configuration to use</returns>
        public virtual ComponentDatabaseConfig OverrideComponentDatabaseConfig() => new ComponentDatabaseConfig() {};

        /// <summary>
        /// Resolve any dependencies the application needs
        /// </summary>
        /// <remarks>By default it will setup IEntityDatabase, IObservableGroupManager and base class dependencies</remarks>
        protected override void ResolveApplicationDependencies()
        {
            base.ResolveApplicationDependencies();
            EntityCollection = DependencyResolver.Resolve<IEntityCollection>();
            ComponentDatabase = DependencyResolver.Resolve<IComponentDatabase>();
            ComputedEntityGroupRegistry = DependencyResolver.Resolve<IComputedEntityGroupRegistry>();
            ComputedComponentGroupRegistry = DependencyResolver.Resolve<IComputedComponentGroupRegistry>();
        }
    }
}