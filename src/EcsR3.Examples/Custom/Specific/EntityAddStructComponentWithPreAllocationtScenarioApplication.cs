using System;
using System.Collections.Generic;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Struct;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.Playground.Specific;

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
        var structComponent1TypeId = ComponentTypeLookup.GetComponentTypeId(typeof(StructComponent1));
        for (var i = 0; i < AllocationAmount; i++)
        {
            var entity = EntityCollection.Create();
            entity.AddComponent<StructComponent1>(structComponent1TypeId);
        }
    }
}