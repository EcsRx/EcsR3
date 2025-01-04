using System.Collections.Generic;
using SystemsR3.Events;
using SystemsR3.Executor;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Plugins;

namespace SystemsR3.Infrastructure
{
    /// <summary>
    /// Acts as an entry point and bootstrapper for the framework
    /// </summary>
    public interface ISystemsR3Application
    {
        /// <summary>
        /// The dependency injection registry
        /// </summary>
        /// <remarks>This will abstract away the underlying DI system, in most cases you wont need it</remarks>
        IDependencyRegistry DependencyRegistry { get; }
        
        /// <summary>
        /// The dependency injection resolver
        /// </summary>
        /// <remarks>This will abstract away the underlying DI system, in most cases you wont need it</remarks>
        IDependencyResolver DependencyResolver { get; }
        
        /// <summary>
        /// The system executor, this orchestrates the systems
        /// </summary>
        ISystemExecutor SystemExecutor { get; }
        
        /// <summary>
        /// The event system to publish and subscribe to events
        /// </summary>
        IEventSystem EventSystem { get; }

        /// <summary>
        /// Any plugins which have been registered within the application
        /// </summary>
        IEnumerable<ISystemsR3Plugin> Plugins { get; }
        
        /// <summary>
        /// This starts the application initialization process
        /// </summary>
        void StartApplication();

        /// <summary>
        /// This stops the application process
        /// </summary>
        void StopApplication();
    }
}