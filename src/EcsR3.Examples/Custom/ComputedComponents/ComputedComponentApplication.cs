using System;
using System.Diagnostics;
using EcsR3.Examples.Application;
using EcsR3.Examples.Custom.ComputedComponents.Blueprints;
using EcsR3.Examples.Custom.ComputedComponents.Components;
using EcsR3.Examples.Custom.ComputedComponents.Computeds;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.ComputedComponents;

public class ComputedComponentApplication : EcsR3ConsoleApplication
{
    protected override void ApplicationStarted()
    {
        var entities = EntityCollection.CreateMany<ComputedComponentBlueprint>(EntityComponentAccessor, 100000);

        var componentGroup = ComputedComponentGroupRegistry.GetComputedGroup<NumberComponent, Number2Component>();
        componentGroup.ForceRefresh();
            
        var computed = new ComputedComponentProcessor(ComponentDatabase, componentGroup);
        
        Console.WriteLine("Starting");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        for (var i = 0; i < 100; i++)
        {
            computed.ForceRefresh();
            Console.WriteLine($"Computed Value: {computed.ComputedData}");
        }
        stopwatch.Stop();
        Console.WriteLine($"Time Taken: {stopwatch.ElapsedMilliseconds}ms");
    }
}