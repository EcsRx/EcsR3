using System;
using System.Diagnostics;
using EcsR3.Blueprints;
using EcsR3.Components.Database;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Struct;
using EcsR3.Examples.ExampleApps.Performance.Systems;
using EcsR3.Extensions;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Pools.Config;
using SystemsR3.Systems;

namespace EcsR3.Examples.ExampleApps.Performance;

public class MakingLotsOfStructEntitiesApplication : EcsR3ConsoleApplication
{
    class LotsOfStructEntitiesBlueprint : IBatchedBlueprint
    {
        public void Apply(IEntityComponentAccessor entityComponentAccessor, int[] entityIds)
        {
            entityComponentAccessor.CreateComponents<StructComponent1, StructComponent2>(entityIds);
        }
    }
        
    private static readonly int EntityCount = 100000;

    public override ComponentDatabaseConfig GetComponentDatabaseConfig => new()
    {
        PoolSpecificConfig =
        {
            {typeof(StructComponent1), new PoolConfig(EntityCount) },
            {typeof(StructComponent2), new PoolConfig(EntityCount) }
        }
    };

    protected override void ApplicationStarted()
    {
        EntityAllocationDatabase.PreAllocate(EntityCount);
        
        Console.WriteLine($"Starting");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        EntityCollection.CreateMany<LotsOfStructEntitiesBlueprint>(EntityComponentAccessor, EntityCount);
        stopwatch.Stop();
        Console.WriteLine($"Finished In: {stopwatch.ElapsedMilliseconds}ms");
    }
}