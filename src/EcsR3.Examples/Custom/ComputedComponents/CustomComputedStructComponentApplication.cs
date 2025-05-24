using System;
using System.Diagnostics;
using EcsR3.Examples.Application;
using EcsR3.Examples.Custom.ComputedComponents.Blueprints;
using EcsR3.Examples.Custom.ComputedComponents.Components;
using EcsR3.Examples.Custom.ComputedComponents.Computeds;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.ComputedComponents;

public class CustomComputedStructComponentApplication : EcsR3ConsoleApplication
{
    protected override void ApplicationStarted()
    {
        var entities = EntityCollection.CreateMany<ComputedStructComponentBlueprint>(100000);

        var componentGroup = ComputedComponentGroupRegistry.GetComputedGroup<StructNumberComponent, StructNumber2Component>();
        componentGroup.RefreshData();
            
        var computed = new CustomComputedStructComponentProcessor(ComponentDatabase, componentGroup);
        
        Console.WriteLine("Starting");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        for (var i = 0; i < 100; i++)
        {
            computed.RefreshData();
            Console.WriteLine($"Computed Value: {computed.ComputedData}");
        }
        stopwatch.Stop();
        Console.WriteLine($"Time Taken: {stopwatch.ElapsedMilliseconds}ms");
    }
}