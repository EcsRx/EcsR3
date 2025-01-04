using System;
using System.Collections;
using DryIoc;
using SystemsR3.Infrastructure.Dependencies;

namespace SystemsR3.Infrastructure.DryIoc
{
    public class DryIocDependencyResolver : IDependencyResolver
    {
        private readonly Container _container;
        public object NativeResolver => _container;

        public DryIocDependencyResolver(Container container)
        { _container = container; }
        
        public object Resolve(Type type, string name = null)
        {
            return string.IsNullOrEmpty(name) ? 
                _container.Resolve(type) : 
                _container.Resolve(type, name);
        }
        
        public IEnumerable ResolveAll(Type type)
        { return _container.ResolveMany(type); }

        public void Dispose()
        {
            // container dispose handled in registry
        }
    }
}