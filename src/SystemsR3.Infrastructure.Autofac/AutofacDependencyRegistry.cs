using System;
using Autofac;
using Autofac.Core;
using SystemsR3.Infrastructure.Dependencies;

namespace SystemsR3.Infrastructure.Autofac
{
    /// <summary>
    /// This is an autofac implementation for the dependency registry.
    /// </summary>
    public class AutofacDependencyRegistry : IDependencyRegistry
    {
        private readonly ContainerBuilder _containerBuilder;
        private IDependencyResolver _resolver;
        public object NativeRegistry => _containerBuilder;

        public AutofacDependencyRegistry(ContainerBuilder containerBuilder = null)
        {
            _containerBuilder = containerBuilder ?? new ContainerBuilder();
        }

        public void Bind(Type fromType, Type toType, BindingConfiguration configuration = null)
        {
            if (configuration == null)
            {
                _containerBuilder.RegisterType(toType).As(fromType).SingleInstance();
                return;
            }

            if (configuration.ToInstance != null)
            {
                var builder = _containerBuilder.RegisterInstance(configuration.ToInstance).As(fromType);
                    
                if(!string.IsNullOrEmpty(configuration.WithName))
                { builder.Named(configuration.WithName, fromType); }

                builder.ExternallyOwned();
            }
            else if (configuration.ToMethod != null)
            {
                var builder = _containerBuilder.Register(x => configuration.ToMethod(_resolver)).As(fromType);
                    
                if(!string.IsNullOrEmpty(configuration.WithName))
                { builder.Named(configuration.WithName, fromType); }
                    
                if (configuration.AsSingleton)
                { builder.SingleInstance(); }
                else
                { builder.InstancePerDependency(); }
            }
            else
            {
                var builder = _containerBuilder.RegisterType(toType).As(fromType);
                    
                if(!string.IsNullOrEmpty(configuration.WithName))
                { builder.Named(configuration.WithName, fromType); }
                    
                if (configuration.AsSingleton)
                { builder.SingleInstance(); }
                else
                { builder.InstancePerDependency(); }
            }
        }

        public void Bind(Type type, BindingConfiguration configuration = null)
        { Bind(type, type, configuration); }

        public bool HasBinding(Type type, string name = null)
        { return _containerBuilder.ComponentRegistryBuilder.IsRegistered(new TypedService(type)); }

        public void Unbind(Type type)
        {
            // This seems unsupported in autofac
        }

        public void LoadModule(IDependencyModule module)
        { module.Setup(this); }

        public IDependencyResolver BuildResolver()
        {
            var container = _containerBuilder.Build();
            _resolver = new AutofacDependencyResolver(container);
            return _resolver;
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}