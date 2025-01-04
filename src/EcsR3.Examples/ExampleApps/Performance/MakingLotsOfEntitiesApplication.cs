using System;
using System.Diagnostics;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Systems;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components;
using EcsR3.Examples.ExampleApps.Performance.Systems;
using EcsR3.Extensions;
using EcsR3.Systems;

namespace EcsR3.Examples.ExampleApps.Performance
{
    public class MakingLotsOfEntitiesApplication : EcsR3ConsoleApplication
    {
        private static readonly int EntityCount = 100000;

        protected override void BindSystems()
        {
            DependencyRegistry.Bind<ISystem, ExampleBatchedSystem>();
        }

        protected override void ApplicationStarted()
        {
            var collection = EntityDatabase.GetCollection();
            
           
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = collection.CreateEntity();
                entity.AddComponents(new SimpleReadComponent(), new SimpleWriteComponent());
            }
            stopwatch.Stop();
            Console.WriteLine($"Finished In: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}