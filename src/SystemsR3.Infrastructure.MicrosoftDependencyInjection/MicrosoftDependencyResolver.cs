using System;
using System.Collections;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SystemsR3.Infrastructure.Dependencies;

namespace SystemsR3.Infrastructure.MicrosoftDependencyInjection
{
    /// <summary>
    /// This is a microsoft di implementation for the dependency resolver
    /// </summary>
    public class MicrosoftDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public object NativeResolver => _serviceProvider;

        public MicrosoftDependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public IEnumerable ResolveAll(Type type)
        { 
            var services = _serviceProvider.GetServices(type); 
            return services;
        }

        public object Resolve(Type type, string name = null)
        {
            if (string.IsNullOrEmpty(name))
            { return _serviceProvider.GetService(type); }
            
            var matchingServices = _serviceProvider.GetKeyedServices(type, name);
            return matchingServices?.First();
        }

        public void Dispose()
        {
            if(_serviceProvider is IDisposable disposable)
            { disposable.Dispose(); }
        }
    }
}