using System;
using System.Diagnostics;
using EcsR3.Components.Database;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Systems;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Examples.ExampleApps.Performance.Systems;
using EcsR3.Extensions;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace EcsR3.Examples.ExampleApps.Performance
{
    public class MakingLotsOfEntitiesApplication : EcsR3ConsoleApplication
    {
        private static readonly int EntityCount = 100000;

        public override ComponentDatabaseConfig GetComponentDatabaseConfig => new ComponentDatabaseConfig()
        {
            PoolSpecificConfig =
            {
                {typeof(ClassComponent1), new PoolConfig(100000) },
                {typeof(ClassComponent2), new PoolConfig(100000) }
            }
        };

        protected override void BindSystems()
        {
            DependencyRegistry.Bind<ISystem, ExampleBatchedSystem>();
        }

        protected override void ApplicationStarted()
        {
            Console.WriteLine($"Starting");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = EntityCollection.Create();
                entity.AddComponents(new ClassComponent1(), new ClassComponent2());
            }
            stopwatch.Stop();
            Console.WriteLine($"Finished In: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}