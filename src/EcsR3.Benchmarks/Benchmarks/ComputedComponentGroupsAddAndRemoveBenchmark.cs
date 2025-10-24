using BenchmarkDotNet.Attributes;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Extensions;
using ClassComponent2 = EcsR3.Examples.Custom.BatchTests.Components.ClassComponent2;

namespace EcsR3.Benchmarks.Benchmarks;

[BenchmarkCategory("Groups")]
public class ComputedComponentGroupsAddAndRemoveBenchmark : EcsR3Benchmark
{
    [Params(1000, 10000)]
    public int EntityCount;

    [Params(5, 10, 20)] 
    public int Iterations;
        
    public override void Setup()
    {
        ComputedComponentGroupRegistry.GetComputedGroup<ClassComponent1, ClassComponent2, ClassComponent3, ClassComponent4>();
    }

    public override void Cleanup()
    {
        EntityCollection.Clear();
    }

    [Benchmark]
    public void ComputedComponentGroupAddRemove()
    {
        for (var j = 0; j < Iterations; j++)
        {
            var entities = EntityCollection.CreateMany(EntityCount);
            EntityComponentAccessor.CreateComponents<ClassComponent1, ClassComponent2, ClassComponent3, ClassComponent4>(entities);
            EntityComponentAccessor.RemoveAllComponents(entities);
            EntityCollection.Clear();
        }
    }
}