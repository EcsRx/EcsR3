using System;
using System.Collections.Generic;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.Specific;

public class EntityAddClassComponentWithPreAllocationtScenarioApplication : EcsR3BenchmarkConsoleApplication
{
    public int AllocationAmount = 100000;
    
    protected override IEnumerable<Type> SpecifyComponentsToIncludeInPool()
    {
        return new[] { typeof(ClassComponent1) };
    }
    
    protected override void ApplicationStarted()
    {
        GetPoolFor<ClassComponent1>().Expand(AllocationAmount);
        
        for (var i = 0; i < AllocationAmount; i++)
        {
            var entityId = EntityCollection.Create();
            EntityComponentAccessor.AddComponent<ClassComponent1>(entityId);
        }
    }
}