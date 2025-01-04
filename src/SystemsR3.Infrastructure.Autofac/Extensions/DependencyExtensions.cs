using Autofac;
using SystemsR3.Infrastructure.Dependencies;

namespace SystemsR3.Infrastructure.Autofac.Extensions
{
    public static class DependencyExtensions
    {
        public static ContainerBuilder GetContainerBuilder(this IDependencyRegistry registry)
        { return registry.NativeRegistry as ContainerBuilder; }
        
        public static IContainer GetContainer(this IDependencyResolver resolver)
        { return resolver.NativeResolver as IContainer; }
    }
}