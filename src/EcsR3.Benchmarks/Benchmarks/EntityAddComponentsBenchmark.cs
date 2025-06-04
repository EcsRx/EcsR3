using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsR3.Components;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Extensions;
using SystemsR3.Extensions;

namespace EcsR3.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Entities")]
    public class EntityAddComponentsBenchmark : EcsR3Benchmark
    {
        private Type[] _availableComponentTypes;
        private List<Entity> _entities = new List<Entity>();
        private IComponent[] _components;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        
        [Params(1000)]
        public int EntityCount;

        [Params(1, 25 ,50)]
        public int ComponentCount;

        public EntityAddComponentsBenchmark()
        {
            var componentNamespace = typeof(ClassComponent1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
        }
        
        public override void Setup()
        {
            for (var i = 0; i < EntityCount; i++)
            { _entities.Add(new Entity(i, 0)); }

            _components = _availableComponentTypes
                .Take(ComponentCount)
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
        }

        public override void Cleanup()
        {
            _entities.ForEach(x => EntityComponentAccessor.RemoveAllComponents(x));
            _entities.Clear();
        }

        [Benchmark]
        public void EntitiesBatchAddComponents()
        {
            _entities.ForEach(x => EntityComponentAccessor.AddComponents(x, _components));
        }
        
        [Benchmark]
        public void EntitiesAddIndividualComponents()
        {
            _entities.ForEach(x => _components.ForEachRun(y => EntityComponentAccessor.AddComponents(x, y)));
        }
    }
}