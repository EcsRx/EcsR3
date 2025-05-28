using System;
using System.Diagnostics;
using EcsR3.Examples.Application;
using EcsR3.Examples.Custom.ComputedComponents.Blueprints;
using EcsR3.Examples.Custom.ComputedComponents.Components;
using EcsR3.Examples.Custom.ComputedComponents.Computeds;
using EcsR3.Extensions;
using EcsR3.Groups;

namespace EcsR3.Examples.Custom.ComputedComponents;

public class ComputedEntityApplication : EcsR3ConsoleApplication
{
    protected override void ApplicationStarted()
    {
        var entities = EntityCollection.CreateMany<ComputedComponentBlueprint>(100000);

        var componentGroup = ComputedEntityGroupRegistry.GetComputedGroup(new Group(typeof(NumberComponent), typeof(Number2Component)));
        var computed = new ComputedEntityProcessor(EntityComponentAccessor, componentGroup);

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