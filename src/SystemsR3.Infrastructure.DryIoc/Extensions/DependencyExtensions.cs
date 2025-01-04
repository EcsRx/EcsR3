using DryIoc;
using SystemsR3.Infrastructure.Dependencies;

namespace SystemsR3.Infrastructure.DryIoc.Extensions
{
    public static class DependencyExtensions
    {
        public static Container GetContainer(this IDependencyRegistry registry)
        { return registry.NativeRegistry as Container; }
        
        public static Container GetContainer(this IDependencyResolver resolver)
        { return resolver.NativeResolver as Container; }
    }
}