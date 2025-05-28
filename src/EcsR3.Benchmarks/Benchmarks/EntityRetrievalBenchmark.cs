using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsR3.Components;
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
        private List<int> _entities = new List<int>();
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

        public void ProcessEntity(int entityId)
        {
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

        public override void Setup()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                EntityComponentAccessor.AddComponents(i, _availableComponents);
                _entities.Add(i);
            }
        }

        public override void Cleanup()
        {
            _entities.ForEach(x => EntityComponentAccessor.RemoveAllComponents(x));
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