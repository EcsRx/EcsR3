using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsR3.Collections.Entity;
using EcsR3.Components;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Performance.Components.Specific;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using SystemsR3.Extensions;

namespace EcsR3.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Groups")]
    public class ObservableGroupsAddAndRemoveBenchmark : EcsR3Benchmark
    {
        private IComponent[] _availableComponents;
        private Type[] _availableComponentTypes;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        
        [Params(1000)]
        public int EntityCount;
        
        [Params(10, 20, 50)]
        public int ComponentCount;

        public ObservableGroupsAddAndRemoveBenchmark() : base()
        {
            var componentNamespace = typeof(Component1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
        }

        public override void Setup()
        {
            var group = new Group(_availableComponentTypes.Take(ComponentCount).ToArray());
            ObservableGroupManager.GetObservableGroup(group);
            
            _availableComponents = _availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .Take(ComponentCount)
                .ToArray();
        }

        public override void Cleanup()
        {
            EntityCollection.RemoveAllEntities();
        }

        [Benchmark]
        public void ObservableGroupAddRemove()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = EntityCollection.CreateEntity();
                entity.AddComponents(_availableComponents);
                entity.RemoveAllComponents();
            }
        }
    }
}