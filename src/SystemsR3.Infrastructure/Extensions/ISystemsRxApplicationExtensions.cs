using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SystemsR3.Extensions;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Systems;

namespace SystemsR3.Infrastructure.Extensions
{
    public static class ISystemsRxApplicationExtensions
    {
        /// <summary>
        /// This will bind any ISystem implementations found within Systems, ViewResolvers folders which are located
        /// in a child namespace of the application.
        /// </summary>
        /// <remarks>
        /// This is a conventional based binding that expects the application file to sit in the root of a directory,
        /// and then have systems in folders within same directory,if you need other conventions then look at wrapping
        /// BindAnySystemsInNamespace
        /// </remarks>
        /// <param name="application">The application to act on</param>
        public static void BindAllSystemsWithinApplicationScope(this ISystemsR3Application application)
        {
            var applicationNamespace = application.GetType().Namespace;
            var namespaces = new[]
            {
                $"{applicationNamespace}.Systems",
                $"{applicationNamespace}.ViewResolvers"
            };
            
            application.DependencyRegistry.BindAllSystemsInNamespaces(namespaces);
        }
        
        /// <summary>
        /// This will bind the given system type (T) to the DI container against `ISystem`
        /// and will then immediately register the system with the SystemExecutor.
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <typeparam name="T">The implementation of ISystem to bind/register</typeparam>
        /// <remarks>This is really for runtime usage, in mose cases you will want to bind in starting and register in started</remarks>
        public static void BindAndStartSystem<T>(this ISystemsR3Application application) where T : ISystem
        {
            application.DependencyRegistry.Bind<ISystem, T>(x => x.WithName(typeof(T).Name));
            StartSystem<T>(application);
        }

        /// <summary>
        /// This will resolve the given type (T) from the DI container then register it
        /// with the SystemExecutor.
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <typeparam name="T">The implementation of ISystem to register</typeparam>
        public static void StartSystem<T>(this ISystemsR3Application application) where T : ISystem
        {
            var groupSystem = application.DependencyResolver.Resolve<ISystem>(typeof(T).Name) 
                              ?? application.DependencyResolver.Resolve<T>();
            
            application.SystemExecutor.AddSystem(groupSystem);
        }
        
        /// <summary>
        /// Resolve all systems which have been bound and register them in order with the systems executor
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <remarks>Will be purely</remarks>
        public static void StartAllBoundSystems(this ISystemsR3Application application)
        {
            var allSystems = application.DependencyResolver.GetAllBoundSystems();
            allSystems.ForEachRun(application.SystemExecutor.AddSystem);
        }
        
        /// <summary>
        /// Gets all known system within a given application scope
        /// </summary>
        /// <param name="application">The application to scope to</param>
        /// <returns>Returns any systems within known conventions folders (i.e /Systems or /ViewResolvers)</returns>
        public static IEnumerable<ISystem> GetAllSystemsWithinApplicationScope(this ISystemsR3Application application)
        {
            var applicationNamespace = application.GetType().Namespace;
            var namespaces = new[]
            {
                $"{applicationNamespace}.Systems",
                $"{applicationNamespace}.ViewResolvers"
            };
            
            return application.DependencyResolver.GetAllSystemsInNamespaces(namespaces);
        }

    }
}