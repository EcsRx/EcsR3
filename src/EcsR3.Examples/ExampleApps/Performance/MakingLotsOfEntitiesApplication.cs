using System;
using System.Diagnostics;
using EcsR3.Blueprints;
using EcsR3.Components.Database;
using EcsR3.Entities.Accessors;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Systems;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Examples.ExampleApps.Performance.Systems;
using EcsR3.Extensions;
using SystemsR3.Pools.Config;

namespace EcsR3.Examples.ExampleApps.Performance
{
    public class MakingLotsOfEntitiesApplication : EcsR3ConsoleApplication
    {
        class LotsOfEntitiesBlueprint : IBlueprint
        {
            public void Apply(IEntityComponentAccessor entityComponentAccessor, int entityId)
            {
                entityComponentAccessor.AddComponents(entityId, new ClassComponent1(), new ClassComponent2());
            }
        }
        
        private static readonly int EntityCount = 100000;

        public override ComponentDatabaseConfig GetComponentDatabaseConfig => new()
        {
            PoolSpecificConfig =
            {
                {typeof(ClassComponent1), new PoolConfig(EntityCount) },
                {typeof(ClassComponent2), new PoolConfig(EntityCount) }
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
            EntityCollection.CreateMany<LotsOfEntitiesBlueprint>(EntityComponentAccessor, EntityCount);
            stopwatch.Stop();
            Console.WriteLine($"Finished In: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}