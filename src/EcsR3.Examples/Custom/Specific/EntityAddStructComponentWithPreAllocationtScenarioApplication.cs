using System;
using System.Collections.Generic;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Struct;

namespace EcsR3.Examples.Custom.Specific;

public class EntityAddStructComponentWithPreAllocationtScenarioApplication : EcsR3BenchmarkConsoleApplication
{
    public int AllocationAmount = 100000;
    
    protected override IEnumerable<Type> SpecifyComponentsToIncludeInPool()
    {
        return new[] { typeof(StructComponent1) };
    }
    
    protected override void ApplicationStarted()
    {
        GetPoolFor<StructComponent1>().Expand(AllocationAmount);
        for (var i = 0; i < AllocationAmount; i++)
        {
            var entityId = EntityCollection.Create();
            EntityComponentAccessor.CreateComponent<StructComponent1>(entityId);
        }
    }
}