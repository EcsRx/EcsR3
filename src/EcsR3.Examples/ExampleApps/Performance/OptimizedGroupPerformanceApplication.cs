using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SystemsR3.Extensions;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Components;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Examples.ExampleApps.Performance.Modules;
using EcsR3.Extensions;
using EcsR3.Groups.Observable;
using SystemsR3.Infrastructure.Modules;

namespace EcsR3.Examples.ExampleApps.Performance
{
    public class OptimizedGroupPerformanceApplication : EcsRxConsoleApplication
    {
        private const int ProcessCount = 10000;
        
        private IComponent[] _availableComponents;
        private int[] _availableComponentTypeIds;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();

        protected override void LoadModules()
        {
            DependencyRegistry.LoadModule<FrameworkModule>();
            DependencyRegistry.LoadModule<OptimizedEcsRxInfrastructureModule>();
        }

        protected override void BindSystems()
        {}

        protected override void ApplicationStarted()
        {
            _availableComponents = _groupFactory.GetComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
         
            _availableComponentTypeIds = Enumerable.Range(0, _availableComponents.Length-1).ToArray();
            
            var groups = _groupFactory.CreateTestGroups(10).ToArray();
            var observableGroups = new List<IObservableGroup>();
            foreach (var group in groups)
            {
                var newGroup = ObservableGroupManager.GetObservableGroup(group);
                observableGroups.Add(newGroup);
            }

            var firstRun = ProcessEntities(ProcessCount);
            var secondRun = ProcessEntities(ProcessCount);
            var thirdRun = ProcessEntities(ProcessCount);

            Console.WriteLine($"Processing with {_availableComponents.Length} components and {observableGroups.Count} Observable groups");
            Console.WriteLine($"Finished In: {(firstRun + secondRun + thirdRun).TotalSeconds}s");
            Console.WriteLine($"First Took: {firstRun.TotalSeconds}s");
            Console.WriteLine($"Second Took: {secondRun.TotalSeconds}s");
            Console.WriteLine($"Third Took: {thirdRun.TotalSeconds}s");
        }

        private TimeSpan ProcessEntities(int amount)
        {
            var defaultPool = EntityDatabase.GetCollection();
            EntityDatabase.Collections.ForEachRun(x => x.RemoveAllEntities());
            GC.Collect();
            
            var timer = Stopwatch.StartNew();

            for (var i = 0; i < amount; i++)
            {
                var entity = defaultPool.CreateEntity();
                entity.AddComponents(_availableComponents);
                entity.RemoveComponents(_availableComponentTypeIds);
            }

            timer.Stop();
            return TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
        }
    }
}