using System;
using System.Collections;
using System.Collections.Generic;
using Autofac;
using SystemsR3.Infrastructure.Dependencies;

namespace SystemsR3.Infrastructure.Autofac
{
    /// <summary>
    /// This is a autofac di implementation for the dependency resolver
    /// </summary>
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public object NativeResolver => _container;

        public AutofacDependencyResolver(IContainer container)
        { _container = container; }

        public IEnumerable ResolveAll(Type type)
        {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(type);
            var services = _container.Resolve(enumerableType) as IEnumerable;
            return services;
        }

        public object Resolve(Type type, string name = null)
        { return string.IsNullOrEmpty(name) ? _container.Resolve(type) : _container.ResolveNamed(name, type); }

        public void Dispose()
        { _container.Dispose(); }
    }
}