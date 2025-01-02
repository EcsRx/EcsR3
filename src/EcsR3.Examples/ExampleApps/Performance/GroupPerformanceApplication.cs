using System;
using System.Diagnostics;
using System.Linq;
using SystemsR3.Extensions;
using EcsR3.Components;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.Performance
{
    public class GroupPerformanceApplication : EcsR3ConsoleApplication
    {
        private IComponent[] _availableComponents;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();

        protected override void BindSystems()
        {}
        
        protected override void ApplicationStarted()
        {
            _availableComponents = _groupFactory.GetComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
            
            var groups = _groupFactory.CreateTestGroups().ToArray();
            foreach (var group in groups)
            { ObservableGroupManager.GetObservableGroup(group); }

            var firstRun = ProcessEntities(10000);
            var secondRun = ProcessEntities(10000);
            var thirdRun = ProcessEntities(10000);

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
                entity.RemoveComponents(_availableComponents);
            }

            timer.Stop();
            return TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
        }
    }
}