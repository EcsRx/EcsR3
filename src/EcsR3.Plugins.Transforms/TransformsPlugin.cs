using System;
using System.Collections.Generic;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Plugins;
using SystemsR3.Systems;

namespace EcsR3.Plugins.Transforms
{
    public class TransformsPlugin : ISystemsRxPlugin
    {
        public string Name => "EcsR3 Transforms";
        public Version Version { get; } = new Version("1.0.0");

        public void SetupDependencies(IDependencyRegistry registry)
        {
            // Nothing needs registering
        }
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyResolver resolver) => Array.Empty<ISystem>();
    }
}