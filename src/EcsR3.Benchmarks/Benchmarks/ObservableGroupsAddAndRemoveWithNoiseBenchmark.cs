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
    public class ObservableGroupsAddAndRemoveWithNoiseBenchmark : EcsR3Benchmark
    {
        private IComponent[] _availableComponents;
        private IComponent[] _noiseComponents;
        private Type[] _availableComponentTypes;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();

        [Params(1000)]
        public int EntityCount;
        
        [Params(10, 20, 50)]
        public int ComponentCount;
        
        [Params(10, 20, 50)]
        public int NoiseAmount;

        public ObservableGroupsAddAndRemoveWithNoiseBenchmark() : base()
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

            var componentsToUse = _availableComponentTypes
                .Take(ComponentCount);

            var noiseComponents = _availableComponentTypes
                .Where(x => !componentsToUse.Contains(x))
                .Take(NoiseAmount);

            _availableComponents = componentsToUse
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
            
            _noiseComponents = noiseComponents
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
        }

        public override void Cleanup()
        {
            EntityCollection.RemoveAll();
        }

        [Benchmark]
        public void ObservableGroupAddRemove()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = EntityCollection.Create();
                entity.AddComponents(_availableComponents);
                entity.AddComponents(_noiseComponents);
                entity.RemoveAllComponents();
            }
        }
    }
}