using SystemsR3.Infrastructure;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Collections;
using EcsR3.Collections.Database;
using EcsR3.Infrastructure.Modules;

namespace EcsR3.Infrastructure
{
    public abstract class EcsRxApplication : SystemsRxApplication, IEcsRxApplication
    {
        public IEntityDatabase EntityDatabase { get; private set; }
        public IObservableGroupManager ObservableGroupManager { get; private set; }
        
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
            DependencyRegistry.LoadModule(new EcsRxInfrastructureModule());
        }

        /// <summary>
        /// Resolve any dependencies the application needs
        /// </summary>
        /// <remarks>By default it will setup IEntityDatabase, IObservableGroupManager and base class dependencies</remarks>
        protected override void ResolveApplicationDependencies()
        {
            base.ResolveApplicationDependencies();
            EntityDatabase = DependencyResolver.Resolve<IEntityDatabase>();
            ObservableGroupManager = DependencyResolver.Resolve<IObservableGroupManager>();
        }
    }
}