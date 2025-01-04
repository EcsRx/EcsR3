using System.Collections.Generic;
using System.Linq;
using EcsR3.Systems;
using SystemsR3.Extensions;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Systems;

namespace EcsR3.Infrastructure.Extensions
{
    public static class IEcsRxApplicationExtensions
    {
        /// <summary>
        /// Resolve all systems which have been bound in the order they need to be triggered
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <remarks>The ordering here will be Setup, Anything else</remarks>
        public static IEnumerable<ISystem> GetAllBoundReactiveSystems(this IEcsR3Application application)
        {
            var allSystems = application.DependencyResolver.ResolveAll<ISystem>();

            return allSystems
                .OrderByDescending(x => x is ISetupSystem)
                .ThenByPriority();
        }
        
        /// <summary>
        /// Resolve all systems which have been bound and register them in order with the systems executor
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <remarks>The ordering here will be Setup, Anything else</remarks>
        public static void StartAllBoundReactiveSystems(this IEcsR3Application application)
        {
            var orderedSystems = GetAllBoundReactiveSystems(application);
            orderedSystems.ForEachRun(application.SystemExecutor.AddSystem);
        }
    }
}