using System;
using SystemsR3.Infrastructure.Exceptions;

namespace SystemsR3.Infrastructure.Dependencies
{
    public class BindingBuilder
    {
        protected readonly BindingConfiguration _configuration = new BindingConfiguration();

        public BindingBuilder AsSingleton()
        {
            _configuration.AsSingleton = true;
            return this;
        }

        public BindingBuilder AsTransient()
        {
            _configuration.AsSingleton = false;
            return this;
        }

        public BindingBuilder WithName(string name)
        {
            _configuration.WithName = name;
            return this;
        }       
        
        internal BindingConfiguration Build()
        {
            return _configuration;
        }
    }
    
    public class BindingBuilder<TFrom> : BindingBuilder
    {
        public BindingBuilder<TFrom> ToInstance<TTo>(TTo instance) where TTo : TFrom
        {
            if(_configuration.ToMethod != null)
            { throw new BindingException("Cannot use instance when a method has been provided already"); }
            
            _configuration.ToInstance = instance;
            return this;
        }
        
        public BindingBuilder<TFrom> ToMethod<TTo>(Func<IDependencyResolver, TTo> method) where TTo : TFrom
        {
            if(_configuration.ToInstance != null)
            { throw new BindingException("Cannot use method when an instance has been provided already"); }
            
            _configuration.ToMethod = container => method(container);
            return this;
        }

        public BindingBuilder<TFrom> ToBoundType<TTo>() where TTo : TFrom
        { return ToBoundType(typeof(TTo));  }
        
        public BindingBuilder<TFrom> ToBoundType(Type boundType)
        {
            if(_configuration.ToMethod != null)
            { throw new BindingException("Cannot use bound type when a method has been provided already"); }
            
            if(_configuration.ToInstance != null)
            { throw new BindingException("Cannot use bound type when an instance has been provided already"); }
            
            _configuration.ToMethod = container => container.Resolve(boundType);
            return this;
        }
    }
}