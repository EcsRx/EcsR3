using System;
using System.Collections;
using Ninject;
using SystemsR3.Infrastructure.Dependencies;

namespace SystemsR3.Infrastructure.Ninject
{
    /// <summary>
    /// This is a ninject implementation for the dependency resolver
    /// </summary>
    public class NinjectDependencyResolver : IDependencyResolver
    {
        public object NativeResolver => _kernel;

        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }
        
        public IEnumerable ResolveAll(Type type)
        { return _kernel.GetAll(type); }
        
        public object Resolve(Type type, string name = null)
        { return string.IsNullOrEmpty(name) ? _kernel.Get(type) : _kernel.Get(type, name); }

        public void Dispose()
        {
            // Registry disposes Kernel
        }
    }
}