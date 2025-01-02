using System.Collections.Generic;
using System.Linq;
using SystemsR3.Events;
using SystemsR3.Executor;
using SystemsR3.Extensions;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Infrastructure.Modules;
using SystemsR3.Infrastructure.Plugins;
using SystemsR3.Systems;

namespace SystemsR3.Infrastructure
{
    public abstract class SystemsRxApplication : ISystemsRxApplication
    {
        public ISystemExecutor SystemExecutor { get; private set; }
        public IEventSystem EventSystem { get; private set; }
        public IEnumerable<ISystemsRxPlugin> Plugins => _plugins;

        private readonly List<ISystemsRxPlugin> _plugins;

        public abstract IDependencyRegistry DependencyRegistry { get; }
        public IDependencyResolver DependencyResolver { get; protected set; }

        protected SystemsRxApplication()
        {
            _plugins = new List<ISystemsRxPlugin>();
        }

        /// <summary>
        /// This starts the process of initializing the application
        /// </summary>
        public virtual void StartApplication()
        {
            LoadModules();
            LoadPlugins();
            SetupPlugins();
            BindSystems();

            DependencyResolver = DependencyRegistry.BuildResolver();
            ResolveApplicationDependencies();
            StartPluginSystems();
            StartSystems();
            ApplicationStarted();
        }

        /// <summary>
        /// This stops all systems
        /// </summary>
        public virtual void StopApplication()
        { StopSystems(); }

        /// <summary>
        /// Load any modules that your application needs
        /// </summary>
        /// <remarks>
        /// If you wish to use the default setup call through to base, if you wish to stop the default framework
        /// modules loading then do not call base and register your own internal framework module.
        /// </remarks>
        protected virtual void LoadModules()
        {
            DependencyRegistry.LoadModule(new FrameworkModule());
        }

        /// <summary>
        /// Load any plugins that your application needs
        /// </summary>
        /// <remarks>It is recommended you just call RegisterPlugin method in here for each plugin you need</remarks>
        protected virtual void LoadPlugins(){}

        /// <summary>
        /// Resolve any dependencies the application needs
        /// </summary>
        /// <remarks>By default it will setup SystemExecutor, EventSystem, EntityCollectionManager</remarks>
        protected virtual void ResolveApplicationDependencies()
        {
            SystemExecutor = DependencyResolver.Resolve<ISystemExecutor>();
            EventSystem = DependencyResolver.Resolve<IEventSystem>();
        }

        /// <summary>
        /// Bind any systems that the application will need
        /// </summary>
        /// <remarks>By default will auto bind any systems within application scope</remarks>
        protected virtual void BindSystems()
        { this.BindAllSystemsWithinApplicationScope(); }

        protected virtual void StopSystems()
        {
            var allSystems = SystemExecutor.Systems.ToList();
            allSystems.ForEachRun(SystemExecutor.RemoveSystem);
        }

        /// <summary>
        /// Start any systems that the application will need
        /// </summary>
        /// <remarks>By default it will auto start any systems which have been bound</remarks>
        protected virtual void StartSystems()
        { this.StartAllBoundSystems(); }
        
        protected abstract void ApplicationStarted();

        protected void SetupPlugins()
        { Plugins.ForEachRun(x => x.SetupDependencies(DependencyRegistry)); }

        protected void StartPluginSystems()
        {
            Plugins.SelectMany(x => x.GetSystemsForRegistration(DependencyResolver))
                .ForEachRun(SystemExecutor.AddSystem);
        }

        /// <summary>
        /// Register a given plugin within the application
        /// </summary>
        /// <param name="plugin">The plugin to register</param>
        protected void RegisterPlugin(ISystemsRxPlugin plugin)
        { _plugins.Add(plugin); }
    }
}