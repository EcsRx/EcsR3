using System;
using Microsoft.Extensions.DependencyInjection;
using SystemsR3.Infrastructure.Dependencies;

namespace SystemsR3.Infrastructure.MicrosoftDependencyInjection.Extensions
{
    public static class DependencyExtensions
    {
        public static IServiceCollection GetServiceCollection(this IDependencyRegistry registry)
        { return registry.NativeRegistry as IServiceCollection; }
        
        public static IServiceProvider GetServiceProvider(this IDependencyResolver resolver)
        { return resolver.NativeResolver as IServiceProvider; }
    }
}