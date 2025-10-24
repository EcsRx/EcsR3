using System.Numerics;
using BenchmarkDotNet.Attributes;
using EcsR3.Blueprints;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Custom.BatchTests.Components;
using EcsR3.Extensions;
using EcsR3.Systems.Batching.Convention;
using R3;
using SystemsR3.Threading;

namespace EcsR3.Benchmarks.Benchmarks;

[BenchmarkCategory("Systems")]
public class BatchSystemMultiThreadingBenchmark : EcsR3Benchmark
{
    public class ClassBatchSystemBlueprint : IBlueprint, IBatchedBlueprint
    {
        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        { entityComponentAccessor.AddComponents(entity, new ClassComponent(), new ClassComponent2()); }

        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity[] entities)
        { entityComponentAccessor.CreateComponents<ClassComponent, ClassComponent2>(entities); }
    }
    
    public class ClassBatchSystem : BatchedSystem<ClassComponent, ClassComponent2>
    {
        public ClassBatchSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {
        }

        protected override Observable<Unit> ReactWhen()
        { return Observable.Never<Unit>(); }

        public void ForceRun() => ProcessBatch();
        public bool UseMultithreading(bool should) => ShouldMultithread = should;
            
        protected override void Process(Entity entity, ClassComponent component1, ClassComponent2 component2)
        {
            component1.Position += Vector3.One;
            component1.Something += 10;
            component2.IsTrue = true;
            component2.Value += 10;
        }
    }
    
    [Params(true, false)]
    public bool UseMultithreading;
    
    [Params(100, 1000)]
    public int Invocations;
    
    public ClassBatchSystem BatchingSystem { get; private set; }

    public override void Setup()
    {
        BatchingSystem = new ClassBatchSystem(ComponentDatabase, EntityComponentAccessor, ComputedComponentGroupRegistry, new DefaultThreadHandler());
        BatchingSystem.StartSystem();
        EntityCollection.CreateMany<ClassBatchSystemBlueprint>(EntityComponentAccessor, 1000);
    }

    public override void Cleanup()
    {
        EntityCollection.Clear();
        BatchingSystem.StopSystem();
    }

    [Benchmark]
    public void ForceBatchRuns()
    {
        BatchingSystem.UseMultithreading(UseMultithreading);
        for(var i=0;i<Invocations;i++)
        { BatchingSystem.ForceRun(); }
    }
}