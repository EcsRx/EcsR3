using System;
using System.Collections.Generic;
using SystemsR3.Executor.Handlers;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Infrastructure.Plugins;
using SystemsR3.Systems;
using EcsR3.Plugins.GroupBinding.Systems.Handlers;

namespace EcsR3.Plugins.GroupBinding
{
    public class GroupBindingsPlugin : ISystemsR3Plugin
    {
        public string Name => "Group Bindings";
        public Version Version { get; } = new Version("1.0.0");
        
        public void SetupDependencies(IDependencyRegistry registry)
        { registry.Bind<IConventionalSystemHandler, GroupBindingSystemHandler>(); }
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyResolver resolver) => Array.Empty<ISystem>();
    }
}