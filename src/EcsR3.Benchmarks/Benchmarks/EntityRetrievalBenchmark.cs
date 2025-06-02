using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsR3.Components;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Extensions;

namespace EcsR3.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Entities")]
    public class EntityRetrievalBenchmark : EcsR3Benchmark
    {
        private IEnumerable<IComponent> _availableComponents;
        private Type[] _availableComponentTypes;
        private Entity[] _entities;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        
        [Params(1000, 10000, 100000)]
        public int EntityCount;

        public EntityRetrievalBenchmark() : base()
        {
            var componentNamespace = typeof(ClassComponent1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();

            _availableComponents = _availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent);
        }

        public override void Setup()
        {
            _entities = EntityCollection.CreateMany(EntityCount);
            for (var i = 0; i < _entities.Length; i++)
            { EntityComponentAccessor.AddComponents(_entities[i], _availableComponents.ToArray()); }
        }

        public override void Cleanup()
        {
            for (var i = 0; i < _entities.Length; i++)
            { EntityComponentAccessor.RemoveAllComponents(_entities[i]); }
        }

        [Benchmark]
        public void EntitiesGetComponents()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entityId = _entities[i];
                EntityComponentAccessor.GetComponent<ClassComponent1>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent2>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent3>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent4>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent5>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent6>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent7>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent8>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent9>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent10>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent11>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent12>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent13>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent14>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent15>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent16>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent17>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent18>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent19>(entityId);
                EntityComponentAccessor.GetComponent<ClassComponent20>(entityId);
            }
        }
        
        [Benchmark]
        public void EntitiesGetComponents_Batched()
        {
            EntityComponentAccessor.GetComponent<ClassComponent1>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent2>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent3>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent4>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent5>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent6>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent7>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent8>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent9>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent10>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent11>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent12>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent13>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent14>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent15>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent16>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent17>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent18>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent19>(_entities);
            EntityComponentAccessor.GetComponent<ClassComponent20>(_entities);
        }
        
        [Benchmark]
        public bool EntitiesHasAllComponents()
        {
            var ignore = false;
            for (var i = 0; i < EntityCount; i++)
            { ignore = EntityComponentAccessor.HasAllComponents(_entities[i], _availableComponentTypes); }
            return ignore;
        }
        
        [Benchmark]
        public bool EntitiesHasAnyComponents()
        {
            var ignore = false;
            for (var i = 0; i < EntityCount; i++)
            { ignore = EntityComponentAccessor.HasAnyComponents(_entities[i], _availableComponentTypes); }
            return ignore;
        }
    }
}