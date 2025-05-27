using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Performance.Components.Struct;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Extensions;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace EcsR3.Benchmarks.Benchmarks;

[BenchmarkCategory("Entities")]
public class PreAllocated_EntityAdd_StructComponents_Benchmark : EcsR3Benchmark
{
    private Type[] _availableComponentTypes;
    private List<IEntity> _entities = new List<IEntity>();
    private IComponent[] _components;
    private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();

    public override ComponentDatabaseConfig OverrideComponentDatabaseConfig()
    {
        Console.WriteLine($"PreAllocating {EntityCount} entries for {ComponentCount} Components");
        return new ComponentDatabaseConfig()
        {
            OnlyPreAllocatePoolsWithConfig = true,
            PoolSpecificConfig =
            {
                { typeof(StructComponent1), new PoolConfig(EntityCount) },
                { typeof(StructComponent2), new PoolConfig(ComponentCount >= 2 ? EntityCount : 0) },
                { typeof(StructComponent3), new PoolConfig(ComponentCount == 3 ? EntityCount : 0) },
            }
        };
    }

    [Params(100000)]
    public int EntityCount;

    [Params(1, 2 ,3)]
    public int ComponentCount;

    public override void Setup()
    {
        for (var i = 0; i < EntityCount; i++)
        {
            var entity = new Entity(i, EntityComponentAccessor);
            _entities.Add(entity);
        }

        _components = new []{ typeof(StructComponent1), typeof(StructComponent2), typeof(StructComponent3) }
            .Take(ComponentCount)
            .Select(x => Activator.CreateInstance(x) as IComponent)
            .ToArray();
    }

    public override void Cleanup()
    {
        _entities.ForEach(x => x.RemoveAllComponents());
        _entities.Clear();
    }

    [Benchmark]
    public void PreAllocated_EntitiesBatchAdd_StructComponents()
    {
        for (var i = 0; i < _components.Length; i++)
        { _entities[i].AddComponents(_components); }
    }
        
    [Benchmark]
    public void PreAllocated_EntitiesAddIndividual_StructComponents()
    {
        for (var i = 0; i < _components.Length; i++)
        {
            for (var j = 0; j < _components.Length; j++)
            { _entities[i].AddComponent(_components[j]); }
        }
    }
}