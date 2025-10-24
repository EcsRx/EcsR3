using System;
using System.Diagnostics;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.ComputedComponents;

public class ComputedComponentBatchApplication : EcsR3ConsoleApplication
{
    protected override void ApplicationStarted()
    {
        var entityCount = 10000;
        ComputedComponentGroupRegistry.GetComputedGroup<ClassComponent1, ClassComponent2, ClassComponent3, ClassComponent4>();
            
        Console.WriteLine("Starting");
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var entities = EntityCollection.CreateMany(entityCount);
        Console.WriteLine($"Created Entities: {stopwatch.ElapsedMilliseconds}ms");
        EntityComponentAccessor.CreateComponents<ClassComponent1, ClassComponent2, ClassComponent3, ClassComponent4>(entities);
        Console.WriteLine($"Created Components: {stopwatch.ElapsedMilliseconds}ms");
        EntityComponentAccessor.RemoveAllComponents(entities);
        Console.WriteLine($"Removed Components: {stopwatch.ElapsedMilliseconds}ms");
        
        stopwatch.Stop();
        Console.WriteLine($"Time Taken: {stopwatch.ElapsedMilliseconds}ms");
    }
}