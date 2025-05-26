using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsR3.Components;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Extensions;
using EcsR3.Groups;

namespace EcsR3.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Groups")]
    public class ComputedEntityGroupsAddAndRemoveBenchmark : EcsR3Benchmark
    {
        private IComponent[] _availableComponents;
        private Type[] _availableComponentTypes;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        
        [Params(1000)]
        public int EntityCount;
        
        [Params(10, 20, 50)]
        public int ComponentCount;

        public ComputedEntityGroupsAddAndRemoveBenchmark() : base()
        {
            var componentNamespace = typeof(ClassComponent1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
        }

        public override void Setup()
        {
            var group = new Group(_availableComponentTypes.Take(ComponentCount).ToArray());
            ComputedEntityGroupRegistry.GetComputedGroup(group);
            
            _availableComponents = _availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .Take(ComponentCount)
                .ToArray();
        }

        public override void Cleanup()
        {
            EntityCollection.RemoveAll();
        }

        [Benchmark]
        public void ComputedEntityGroupAddRemove()
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