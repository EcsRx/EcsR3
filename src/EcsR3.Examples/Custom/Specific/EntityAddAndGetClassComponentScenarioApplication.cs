using System;
using System.Collections.Generic;
using System.Diagnostics;
using EcsR3.Components.Database;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Extensions;
using SystemsR3.Pools.Config;

namespace EcsR3.Examples.Custom.Specific;

public class EntityAddAndGetClassComponentScenarioApplication : EcsR3BenchmarkConsoleApplication
{
    public int AllocationAmount = 100000;

    public override ComponentDatabaseConfig OverrideComponentDatabaseConfig()
    {
        return new ComponentDatabaseConfig()
        {
            PoolSpecificConfig =
            {
                { typeof(ClassComponent1), new PoolConfig(AllocationAmount) },
                { typeof(ClassComponent2), new PoolConfig(AllocationAmount) },
                { typeof(ClassComponent3), new PoolConfig(AllocationAmount) },
                { typeof(ClassComponent4), new PoolConfig(AllocationAmount) },
                { typeof(ClassComponent5), new PoolConfig(AllocationAmount) },
                { typeof(ClassComponent6), new PoolConfig(AllocationAmount) },
                { typeof(ClassComponent7), new PoolConfig(AllocationAmount) }
            }
        };
    }

    protected override IEnumerable<Type> SpecifyComponentsToIncludeInPool()
    {
        return new[] { typeof(ClassComponent1), typeof(ClassComponent2), typeof(ClassComponent3), 
            typeof(ClassComponent4), typeof(ClassComponent5), typeof(ClassComponent6), typeof(ClassComponent7) };
    }

    protected override void ApplicationStarted()
    {
        var timer = new Stopwatch();
        Console.WriteLine("STARTING");
        timer.Start();
        
        var entities = EntityCollection.CreateMany(AllocationAmount);
        EntityComponentAccessor.CreateComponents<ClassComponent1,ClassComponent2,ClassComponent3,
            ClassComponent4, ClassComponent5, ClassComponent6, ClassComponent7>(entities);

        EntityComponentAccessor.GetComponent<ClassComponent1>(entities);
        EntityComponentAccessor.GetComponent<ClassComponent2>(entities);
        EntityComponentAccessor.GetComponent<ClassComponent3>(entities);
        EntityComponentAccessor.GetComponent<ClassComponent4>(entities);
        EntityComponentAccessor.GetComponent<ClassComponent5>(entities);
        EntityComponentAccessor.GetComponent<ClassComponent6>(entities);
        EntityComponentAccessor.GetComponent<ClassComponent7>(entities);
        
        timer.Stop();
        Console.WriteLine($"Finished in {timer.ElapsedMilliseconds}ms");
    }
}