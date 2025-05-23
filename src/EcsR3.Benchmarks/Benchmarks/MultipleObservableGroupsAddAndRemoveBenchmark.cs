using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsR3.Collections;
using EcsR3.Components;
using EcsR3.Computeds;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Extensions;
using EcsR3.Groups;
using SystemsR3.Extensions;

namespace EcsR3.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Groups")]
    public class MultipleObservableGroupsAddAndRemoveBenchmark : EcsR3Benchmark
    {
        private IComponent[] _availableComponents;
        private Type[] _availableComponentTypes;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        
        [Params(1000)]
        public int EntityCount;
        
        [Params(1, 10, 25)]
        public int ObservableGroups;

        public MultipleObservableGroupsAddAndRemoveBenchmark() : base()
        {
            var componentNamespace = typeof(ClassComponent1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
        }

        public override void Setup()
        {
            var componentsPerGroup = _availableComponentTypes.Length / ObservableGroups;
            for (var i = 0; i < ObservableGroups; i++)
            {
                var componentsToTake = _availableComponentTypes
                    .Skip(i*(componentsPerGroup))
                    .Take(componentsPerGroup)
                    .ToArray();
                                
                var group = new Group(componentsToTake);
                ComputedEntityGroupRegistry.GetComputedGroup(group);
            }
            
            _availableComponents = _availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
        }

        public override void Cleanup()
        {
            EntityCollection.RemoveAll();
            var manager = (ComputedEntityGroupRegistry as ComputedEntityGroupRegistry);
            var allObservableGroups = manager._computedGroups;
            allObservableGroups.Values.DisposeAll();
            manager._computedGroups.Clear();
        }

        [Benchmark]
        public void MultipleObservableGroupAddRemove()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = EntityCollection.Create();
                entity.AddComponents(_availableComponents);
                entity.RemoveAllComponents();
            }
        }
    }
}