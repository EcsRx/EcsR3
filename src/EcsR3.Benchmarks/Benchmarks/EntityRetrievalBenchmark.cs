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
        private IComponent[] _availableComponents;
        private Type[] _availableComponentTypes;
        private List<IEntity> _entities = new List<IEntity>();
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
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
        }

        public void ProcessEntity(IEntity entity)
        {
            entity.GetComponent<ClassComponent1>();
            entity.GetComponent<ClassComponent2>();
            entity.GetComponent<ClassComponent3>();
            entity.GetComponent<ClassComponent4>();
            entity.GetComponent<ClassComponent5>();
            entity.GetComponent<ClassComponent6>();
            entity.GetComponent<ClassComponent7>();
            entity.GetComponent<ClassComponent8>();
            entity.GetComponent<ClassComponent9>();
            entity.GetComponent<ClassComponent10>();
            entity.GetComponent<ClassComponent11>();
            entity.GetComponent<ClassComponent12>();
            entity.GetComponent<ClassComponent13>();
            entity.GetComponent<ClassComponent14>();
            entity.GetComponent<ClassComponent15>();
            entity.GetComponent<ClassComponent16>();
            entity.GetComponent<ClassComponent17>();
            entity.GetComponent<ClassComponent18>();
            entity.GetComponent<ClassComponent19>();
            entity.GetComponent<ClassComponent20>();
        }

        public override void Setup()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = new Entity(i, EntityComponentAccessor);
                entity.AddComponents(_availableComponents);
                _entities.Add(entity);
            }
        }

        public override void Cleanup()
        {
            _entities.ForEach(x => x.RemoveAllComponents());
            _entities.Clear();
        }

        [Benchmark]
        public void EntitiesGetComponents()
        {
            for (var i = 0; i < EntityCount; i++)
            { ProcessEntity(_entities[i]); }
        }
        
        [Benchmark]
        public bool EntitiesHasAllComponents()
        {
            var ignore = false;
            for (var i = 0; i < EntityCount; i++)
            { ignore = _entities[i].HasAllComponents(_availableComponentTypes); }
            return ignore;
        }
        
        [Benchmark]
        public bool EntitiesHasAnyComponents()
        {
            var ignore = false;
            for (var i = 0; i < EntityCount; i++)
            { ignore = _entities[i].HasAnyComponents(_availableComponentTypes); }
            return ignore;
        }
    }
}