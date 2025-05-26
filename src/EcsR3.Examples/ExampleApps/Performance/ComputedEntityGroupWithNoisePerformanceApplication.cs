using System;
using System.Linq;
using EcsR3.Components;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Groups;

namespace EcsR3.Examples.ExampleApps.Performance;

public class ComputedEntityGroupWithNoisePerformanceApplication : EcsR3ConsoleApplication
{
    private static readonly int EntityCount = 100000;
    private static readonly int Iterations = 3;
        
    protected override void ApplicationStarted()
    {
        var groupFactory = new RandomGroupFactory();
            
        var componentNamespace = typeof(ClassComponent1).Namespace;
        
        var availableComponentTypes = groupFactory.GetComponentTypes
            .Where(x => x.Namespace == componentNamespace)
            .ToArray();
        
        var requiredComponents = availableComponentTypes
            .Take(25)
            .ToArray();
        
        var group = new Group(requiredComponents);
        ComputedEntityGroupRegistry.GetComputedGroup(group);
            
        var availableComponents = availableComponentTypes
            .Select(x => Activator.CreateInstance(x) as IComponent)
            .ToArray();

        for (var iteration = 0; iteration < Iterations; iteration++)
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = EntityCollection.Create();
                entity.AddComponents(availableComponents);
                entity.RemoveAllComponents();
            }
        }
    }
}