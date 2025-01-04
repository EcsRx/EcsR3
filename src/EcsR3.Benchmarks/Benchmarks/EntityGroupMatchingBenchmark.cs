using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsR3.Components;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Performance.Components.Specific;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Extensions;
using EcsR3.Groups;

namespace EcsR3.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Groups", "Entities")]
    public class EntityGroupMatchingBenchmark : EcsR3Benchmark
    {
        private LookupGroup _lookupGroup;
        private List<IEntity> _entities = new List<IEntity>();
        private Type[] _availableComponentTypes;

        private IComponent[] _components;

        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        
        [Params(1000, 10000)]
        public int EntityCount;
        
        [Params(1, 25, 50)]
        public int ComponentCount;


        public EntityGroupMatchingBenchmark() : base()
        {
            var componentNamespace = typeof(Component1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
            
            _lookupGroup = new LookupGroup(_availableComponentTypes
                .Select(ComponentTypeLookup.GetComponentTypeId)
                .ToArray(), Array.Empty<int>());
        }
        
        public override void Setup()
        {
            _components = _availableComponentTypes
                .Take(ComponentCount)
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
            
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = new Entity(i, ComponentDatabase, ComponentTypeLookup);
                entity.AddComponents(_components);
                _entities.Add(entity);
            }
        }

        public override void Cleanup()
        {
            _entities.ForEach(x => x.RemoveAllComponents());
            _entities.Clear();
        }

        [Benchmark]
        public void EntitiesMatchGroup()
        { _entities.ForEach(x => _lookupGroup.Matches(x)); }
    }
}