using System;
using System.Diagnostics;
using EcsR3.Examples.Application;
using EcsR3.Examples.Custom.ComputedComponents.Blueprints;
using EcsR3.Examples.Custom.ComputedComponents.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.ComputedComponents;

public class ComputedComponentGroupTestApplication : EcsR3ConsoleApplication
{
    protected override void ApplicationStarted()
    {
        var entities = EntityCollection.CreateMany<ComputedComponentBlueprint>(100000);

        var componentGroup = ComputedComponentGroupRegistry.GetComputedGroup<NumberComponent, Number2Component>();
        componentGroup.RefreshData();
        
        Console.WriteLine("Starting");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        for (var i = 0; i < 1000; i++)
        { componentGroup.RefreshData(); }
        stopwatch.Stop();
        Console.WriteLine($"Time Taken: {stopwatch.ElapsedMilliseconds}ms");
    }
}