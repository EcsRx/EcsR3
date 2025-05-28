using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components;
using EcsR3.Examples.ExampleApps.Performance.Systems;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.Performance
{
    public class SimpleSystemApplication : EcsR3ConsoleApplication
    {
        private static readonly int EntityCount = 1000;
        private ExampleReactToGroupSystem _groupSystem;

        protected override void ApplicationStarted()
        {
            _groupSystem = new ExampleReactToGroupSystem();
            
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = EntityCollection.Create();
                entity.AddComponents(new SimpleReadComponent(), new SimpleWriteComponent());
            }

            RunSingleThread();
            RunMultiThreaded();
        }

        private void RunSingleThread()
        {
            var timer = Stopwatch.StartNew();
            foreach(var entity in EntityCollection)
            { _groupSystem.Process(EntityComponentAccessor, entity); }
            timer.Stop();

            var totalTime = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
            Console.WriteLine($"Executed {EntityCount} entities in single thread in {totalTime}ms");
        }
        
        private void RunMultiThreaded()
        {
            var timer = Stopwatch.StartNew();
            Parallel.ForEach(EntityCollection, x => _groupSystem.Process(EntityComponentAccessor, x));
            timer.Stop();

            var totalTime = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
            Console.WriteLine($"Executed {EntityCount} entities multi-threaded in {totalTime}ms");
        }
    }
}