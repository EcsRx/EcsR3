using System;
using System.Linq;
using EcsR3.Components;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Groups;

namespace EcsR3.Examples.ExampleApps.Performance
{
    public class ComputedEntityGroupPerformanceApplication : EcsR3ConsoleApplication
    {
        private static readonly int EntityCount = 100000;
        
        protected override void ApplicationStarted()
        {
            var groupFactory = new RandomGroupFactory();
            
            var componentNamespace = typeof(ClassComponent1).Namespace;
            var availableComponentTypes = groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
            
            var observableGroupCount = availableComponentTypes.Length / 2;
            var componentsPerGroup = availableComponentTypes.Length / observableGroupCount;
            for (var i = 0; i < observableGroupCount; i++)
            {
                var componentsToTake = availableComponentTypes
                    .Skip(i*(componentsPerGroup))
                    .Take(componentsPerGroup)
                    .ToArray();
                                
                var group = new Group(componentsToTake);
                ComputedEntityGroupRegistry.GetComputedGroup(group);
            }
            
            var availableComponents = availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();

            for (var i = 0; i < EntityCount; i++)
            {
                var entityId = EntityCollection.Create();
                EntityComponentAccessor.AddComponents(entityId, availableComponents);
                EntityComponentAccessor.RemoveAllComponents(entityId);
            }
        }
    }
}