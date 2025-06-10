using System;
using System.Collections.Generic;
using System.Linq;
using SystemsR3.Extensions;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Systems;

namespace SystemsR3.Infrastructure.Extensions
{
    public static class IDependencyResolverSystemExtensions
    {
        private static readonly Type SystemType = typeof(ISystem);    
        
        /// <summary>
        /// Resolve all systems which have been bound in priority order
        /// </summary>
        /// <param name="resolver">The dependency resolver to act on</param>
        /// <remarks>This ordering will be purely by priority</remarks>
        public static IEnumerable<ISystem> GetAllBoundSystems(this IDependencyResolver resolver)
        {
            var allSystems = resolver.ResolveAll<ISystem>();
            return allSystems.OrderByPriority();
        }
                
        /// <summary>
        /// Gets all systems within a given namespace
        /// </summary>
        /// <param name="resolver">The dependency resolver to use</param>
        /// <param name="namespaces">The namespaces to check within</param>
        /// <returns>An enumerable of all the systems found within that namespace</returns>
        public static IEnumerable<ISystem> GetAllSystemsInNamespaces(this IDependencyResolver resolver, params string[] namespaces)
        {
            var applicationAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var applicableSystems = applicationAssemblies.SelectMany(x => x.GetTypes())
                .Where(x =>
                {                   
                    if(x.IsInterface || x.IsAbstract)
                    { return false; }
                    
                    if(string.IsNullOrEmpty(x.Namespace))
                    { return false; }   
                    
                    if(!SystemType.IsAssignableFrom(x))
                    { return false; }

                    return namespaces.Any(namespaceToVerify => x.Namespace.Contains(namespaceToVerify));
                })
                .ToList();

            if(!applicableSystems.Any())
            { yield break; }

            foreach (var applicableSystemType in applicableSystems)
            { yield return resolver.Resolve<ISystem>(applicableSystemType.Name); }
        }
    }
}