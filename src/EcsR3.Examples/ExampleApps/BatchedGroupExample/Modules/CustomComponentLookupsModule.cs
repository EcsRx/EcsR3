using System;
using System.Collections.Generic;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Components.Lookups;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Components;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Lookups;

namespace EcsR3.Examples.ExampleApps.BatchedGroupExample.Modules
{
    public class CustomComponentLookupsModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            registry.Unbind<IComponentTypeLookup>();
            var explicitTypeLookups = new Dictionary<Type, int>
            {
                {typeof(NameComponent), ComponentLookupTypes.NameComponentId},
                {typeof(PositionComponent), ComponentLookupTypes.PositionComponentId},
                {typeof(MovementSpeedComponent), ComponentLookupTypes.MovementSpeedComponentId}
            };
            var explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups);
            registry.Bind<IComponentTypeLookup>(new BindingConfiguration{ToInstance = explicitComponentLookup});
        }
    }
}