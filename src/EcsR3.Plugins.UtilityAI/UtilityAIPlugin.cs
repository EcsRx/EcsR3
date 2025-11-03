using System;
using System.Collections.Generic;
using EcsR3.Plugins.UtilityAI.Modules;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Infrastructure.Plugins;
using SystemsR3.Systems;

namespace EcsR3.Plugins.UtilityAI
{
    public class UtilityAIPlugin : ISystemsR3Plugin
    {
        public string Name => "Utility AI Plugin";
        public Version Version { get; } = new Version("1.0.0");

        public void SetupDependencies(IDependencyRegistry registry)
        {
            registry.LoadModule<UtilityAIModule>();
        }
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyResolver resolver) => Array.Empty<ISystem>();
    }
}