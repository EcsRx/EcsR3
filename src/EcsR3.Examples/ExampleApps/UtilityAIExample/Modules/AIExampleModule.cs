using System;
using OpenRpg.Core.Utils;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Modules;

public class AIExampleModule : IDependencyModule
{
    public void Setup(IDependencyRegistry registry)
    {
        registry.Bind<IRandomizer>(x => x.ToInstance(new DefaultRandomizer(new Random())));
    }
}