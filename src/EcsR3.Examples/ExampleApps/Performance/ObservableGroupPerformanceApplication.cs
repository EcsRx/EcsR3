using System;
using System.Linq;
using EcsR3.Components;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Specific;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Groups;

namespace EcsR3.Examples.ExampleApps.Performance
{
    public class ObservableGroupPerformanceApplication : EcsR3ConsoleApplication
    {
        private static readonly int EntityCount = 100000;
        
        protected override void ApplicationStarted()
        {
            var groupFactory = new RandomGroupFactory();
            
            var componentNamespace = typeof(Component1).Namespace;
            var availableComponentTypes = groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
            
            var collection = EntityDatabase.GetCollection();
            
            var observableGroupCount = availableComponentTypes.Length / 2;
            var componentsPerGroup = availableComponentTypes.Length / observableGroupCount;
            for (var i = 0; i < observableGroupCount; i++)
            {
                var componentsToTake = availableComponentTypes
                    .Skip(i*(componentsPerGroup))
                    .Take(componentsPerGroup)
                    .ToArray();
                                
                var group = new Group(componentsToTake);
                ObservableGroupManager.GetObservableGroup(group);
            }
            
            var availableComponents = availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();

            for (var i = 0; i < EntityCount; i++)
            {
                var entity = collection.CreateEntity();
                entity.AddComponents(availableComponents);
                entity.RemoveAllComponents();
            }
        }
    }
}