using System;
using System.Collections;

namespace SystemsR3.Infrastructure.Dependencies
{
    /// <summary>
    /// Provides a cross platform way to resolve any dependencies that have been registered
    /// </summary>
    public interface IDependencyResolver : IDisposable
    {
        /// <summary>
        /// This exposes the underlying DI resolver, but any calls to this directly
        /// will not be cross platform, so be weary if you need it or not.
        /// </summary>
        object NativeResolver { get; }

        /// <summary>
        /// Gets an enumerable of a given type from the underlying DI resolver
        /// </summary>
        /// <param name="type">Type to resolve</param>
        /// <returns>All matching instances of that type within the underlying container</returns>
        IEnumerable ResolveAll(Type type);

        /// <summary>
        /// Gets an instance of a given type from the underlying DI container
        /// </summary>
        /// <param name="type">Type of the object you want</param>
        /// <param name="name">Optional name of the binding</param>
        /// <returns>An instance of the given type</returns>
        object Resolve(Type type, string name = null);
    }
}