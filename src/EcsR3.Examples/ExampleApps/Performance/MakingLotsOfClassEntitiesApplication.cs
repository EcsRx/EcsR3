using System;
using System.Diagnostics;
using EcsR3.Blueprints;
using EcsR3.Components.Database;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Extensions;
using SystemsR3.Pools.Config;

namespace EcsR3.Examples.ExampleApps.Performance
{
    public class MakingLotsOfClassEntitiesApplication : EcsR3ConsoleApplication
    {
        class LotsOfClassEntitiesBlueprint : IBatchedBlueprint
        {
            public void Apply(IEntityComponentAccessor entityComponentAccessor, int[] entityIds)
            {
                entityComponentAccessor.CreateComponents<ClassComponent1, ClassComponent2>(entityIds);
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

        protected override void ApplicationStarted()
        {
            EntityAllocationDatabase.PreAllocate(EntityCount);
            
            Console.WriteLine($"Starting");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            EntityCollection.CreateMany<LotsOfClassEntitiesBlueprint>(EntityComponentAccessor, EntityCount);
            stopwatch.Stop();
            Console.WriteLine($"Finished In: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}