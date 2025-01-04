using System;
using SystemsR3.Infrastructure.Dependencies;

namespace SystemsR3.Infrastructure.Extensions
{
    public static class IDependencyRegistryExtensions
    {
        /// <summary>
        /// Binds from one type to another, generally from an interface to a concrete class
        /// </summary>
        /// <param name="registry">The registry to bind on</param>
        /// <param name="configuration">Optional configuration</param>
        /// <typeparam name="TFrom">Type to bind from</typeparam>
        /// <typeparam name="TTo">Type to bind to</typeparam>
        public static void Bind<TFrom, TTo>(this IDependencyRegistry registry, BindingConfiguration configuration = null) where TTo : TFrom
        { registry.Bind(typeof(TFrom), typeof(TTo), configuration); }
        
        
        /// <summary>
        /// Binds from one type to another, generally from an interface to a concrete class
        /// </summary>
        /// <param name="registry">The registry to bind on</param>
        /// <param name="configuration">Optional configuration</param>
        /// <typeparam name="T">Source and possible destination binding, i.e concrete class</typeparam>
        /// <remarks>This is useful for self binding concrete classes</remarks>
        public static void Bind<T>(this IDependencyRegistry registry, BindingConfiguration configuration = null)
        { registry.Bind(typeof(T), configuration); }
        
        /// <summary>
        /// Bind using a builder for fluent configuration
        /// </summary>
        /// <param name="registry">registry to act on</param>
        /// <param name="builderAction">configuration handler</param>
        /// <typeparam name="T">Type to bind to</typeparam>
        public static void Bind<T>(this IDependencyRegistry registry, Action<BindingBuilder<T>> builderAction)
        {
            var builder = new BindingBuilder<T>();
            builderAction(builder);
            var config = builder.Build();
            registry.Bind<T>(config);
        }
        
        /// <summary>
        /// Bind using a builder for fluent configuration
        /// </summary>
        /// <param name="registry">registry to act on</param>
        /// <param name="builderAction">configuration handler</param>
        /// <typeparam name="TFrom">Type to bind from</typeparam>
        /// <typeparam name="TTo">Type to bind to</typeparam>
        public static void Bind<TFrom, TTo>(this IDependencyRegistry registry, Action<BindingBuilder> builderAction) where TTo : TFrom
        {
            var builder = new BindingBuilder();
            builderAction(builder);
            var config = builder.Build();
            registry.Bind<TFrom, TTo>(config);
        }

        /// <summary>
        /// Checks to see if a binding exists in the registry
        /// </summary>
        /// <param name="registry">The registry to check against</param>
        /// <param name="name">Optional name of the binding</param>
        /// <typeparam name="T">Type to check against</typeparam>
        /// <returns>True if the type has been bound, false if not</returns>
        public static bool HasBinding<T>(this IDependencyRegistry registry, string name = null)
        { return registry.HasBinding(typeof(T), name); }
        
        /// <summary>
        /// Unbinds a type from the container
        /// </summary>
        /// <typeparam name="T">The type to unbind</typeparam>
        public static void Unbind<T>(this IDependencyRegistry registry)
        { registry.Unbind(typeof(T)); }

        /// <summary>
        /// Loads the given modules bindings into the underlying di container
        /// </summary>
        /// <typeparam name="T">Type of module to load</typeparam>
        public static void LoadModule<T>(this IDependencyRegistry registry) where T : IDependencyModule, new()
        { registry.LoadModule(new T()); }
    }
}