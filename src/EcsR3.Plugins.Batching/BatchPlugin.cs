using System;
using System.Collections.Generic;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Infrastructure.Plugins;
using SystemsR3.Systems;
using EcsR3.Plugins.Batching.Accessors;
using EcsR3.Plugins.Batching.Factories;

namespace EcsR3.Plugins.Batching
{
    public class BatchPlugin : ISystemsR3Plugin
    {
        public string Name => "Batching";
        public Version Version { get; } = new Version("1.0.0");

        public void SetupDependencies(IDependencyRegistry registry)
        {
            registry.Bind<IBatchBuilderFactory, BatchBuilderFactory>(x => x.AsSingleton());
            registry.Bind<IReferenceBatchBuilderFactory, ReferenceBatchBuilderFactory>(x => x.AsSingleton());
            registry.Bind<IBatchManager, BatchManager>(x => x.AsSingleton());
        }
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyResolver resolver) => Array.Empty<ISystem>();
    }
}