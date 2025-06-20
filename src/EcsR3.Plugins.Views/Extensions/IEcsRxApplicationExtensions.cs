using System.Collections.Generic;
using System.Linq;
using SystemsR3.Extensions;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Systems;
using EcsR3.Infrastructure;
using EcsR3.Plugins.Views.Systems;
using EcsR3.Systems;
using EcsR3.Systems.Reactive;

namespace EcsR3.Plugins.Views.Extensions
{
    public static class IEcsRxApplicationExtensions
    {
        /// <summary>
        /// Resolve all systems which have been bound in the order they need to be triggered
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <remarks>This ordering will be Setup, ViewResolvers, Anything Else</remarks>
        public static IEnumerable<ISystem> GetAllBoundViewSystems(this IEcsR3Application application)
        {
            var allSystems = application.DependencyResolver.ResolveAll<ISystem>();

            return allSystems
                    .OrderByDescending(x => x is ISetupSystem && !(x is IViewResolverSystem))
                    .ThenByDescending(x => x is IViewResolverSystem)
                    .ThenByPriority();
        }
        
        /// <summary>
        /// Resolve all systems which have been bound and register them in order with the systems executor
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// /// <remarks>This ordering will be Setup, ViewResolvers, Anything Else</remarks>
        public static void StartAllBoundViewSystems(this IEcsR3Application application)
        {
            var orderedSystems = GetAllBoundViewSystems(application);
            orderedSystems.ForEachRun(application.SystemExecutor.AddSystem);
        }
    }
}