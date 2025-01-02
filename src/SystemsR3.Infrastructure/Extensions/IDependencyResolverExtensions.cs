using System.Collections.Generic;
using System.Linq;
using SystemsR3.Infrastructure.Dependencies;

namespace SystemsR3.Infrastructure.Extensions
{
    public static class IDependencyResolverExtensions
    {
        /// <summary>
        /// Gets an instance of a given type from the underlying DI container
        /// </summary>
        /// <param name="resolver">The container to resolve from</param>
        /// <param name="name">Optional name of the binding</param>
        /// <typeparam name="T">Type of the object you want</typeparam>
        /// <returns>An instance of the given type</returns>
        public static T Resolve<T>(this IDependencyResolver resolver, string name = null)
        {
            return (T)resolver.Resolve(typeof(T), name);
        }

        /// <summary>
        /// Gets an enumerable of a given type from the underlying DI container
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>All matching instances of that type within the underlying container</returns>
        public static IEnumerable<T> ResolveAll<T>(this IDependencyResolver resolver)
        {
            return resolver.ResolveAll(typeof(T)).Cast<T>();
        }
    }
}