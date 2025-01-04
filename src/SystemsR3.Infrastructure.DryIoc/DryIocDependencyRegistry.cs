using System;
using DryIoc;
using SystemsR3.Infrastructure.Dependencies;

namespace SystemsR3.Infrastructure.DryIoc
{
    public class DryIocDependencyRegistry : IDependencyRegistry
    {
        private readonly Container _container;
        public object NativeRegistry => _container;
        public IDependencyResolver Resolver { get; }
        
        public DryIocDependencyRegistry(Container container = null)
        {
            _container = container ?? new Container();
            Resolver = new DryIocDependencyResolver(_container);
        }

        public static object InitializerDecorator(object instance, IDependencyRegistry container, Action<IDependencyRegistry, object> action)
        {
            action(container, instance);
            return instance;
        }
        
        public void Bind(Type fromType, Type toType, BindingConfiguration configuration = null)
        {
            if (configuration == null)
            {
                _container.Register(fromType, toType, Reuse.Singleton);
                return;
            }
            
            var reuseOption = configuration.AsSingleton ? Reuse.Singleton : Reuse.Transient;
            
            if (configuration.ToInstance != null)
            { _container.RegisterInstance(fromType, configuration.ToInstance, IfAlreadyRegistered.Replace, null, configuration.WithName); }
            else if (configuration.ToMethod != null)
            { _container.RegisterDelegate(fromType, x => configuration.ToMethod(Resolver), null, null, IfAlreadyRegistered.AppendNewImplementation, configuration.WithName); }
            else
            { _container.Register(fromType, toType, reuseOption, null, null, IfAlreadyRegistered.AppendNewImplementation, configuration.WithName); }
        }

        public void Bind(Type type, BindingConfiguration configuration = null)
        { Bind(type, type, configuration); }

        public bool HasBinding(Type type, string name = null)
        {
            return string.IsNullOrEmpty(name) ? 
                _container.IsRegistered(type) : 
                _container.IsRegistered(type, name);
        }

        public void Unbind(Type type)
        { _container.Unregister(type); }

        public void LoadModule(IDependencyModule module)
        { module.Setup(this); }

        public IDependencyResolver BuildResolver()
        { return Resolver; }

        public void Dispose()
        { _container?.Dispose(); }
    }
}