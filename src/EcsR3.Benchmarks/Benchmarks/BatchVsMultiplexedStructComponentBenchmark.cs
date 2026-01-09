using System.Collections.Generic;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using EcsR3.Blueprints;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Custom.BatchTests.Components;
using EcsR3.Examples.ExampleApps.Performance.Components.Struct;
using EcsR3.Extensions;
using EcsR3.Systems.Batching.Convention;
using EcsR3.Systems.Batching.Convention.Multiplexing;
using EcsR3.Systems.Batching.Convention.Multiplexing.Handlers;
using R3;
using SystemsR3.Threading;

namespace EcsR3.Benchmarks.Benchmarks;

[BenchmarkCategory("Systems")]
public class BatchVsMultiplexedStructComponentBenchmark : EcsR3Benchmark
{
    public class BatchVsMultiplexedBlueprint : IBlueprint, IBatchedBlueprint
    {
        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        { entityComponentAccessor.AddComponents(entity, new StructComponent1Multiplexed(), new StructComponent2Multiplexed(), new StructComponent3Multiplexed()); }

        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity[] entities)
        { entityComponentAccessor.CreateComponents<StructComponent1Multiplexed, StructComponent2Multiplexed, StructComponent3Multiplexed>(entities); }
    }
    
#region Batch Systems
    public class ClassBatchSystem1 : BatchedRefSystem<StructComponent1Multiplexed, StructComponent2Multiplexed>
    {
        public ClassBatchSystem1(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen() => Observable.Never<Unit>();
        public void ForceRun() => ProcessBatch();
        public bool UseMultithreading(bool should) => ShouldMultithread = should;
            
        protected override void Process(Entity entity, ref StructComponent1Multiplexed component2, ref StructComponent2Multiplexed component3)
        {
            component2.Position += Vector3.One;
            component2.Something += 10;
            component3.IsTrue = true;
            component3.Value += 10;
        }
    }
    
    public class ClassBatchSystem2 : BatchedRefSystem<StructComponent1Multiplexed, StructComponent3Multiplexed>
    {
        public ClassBatchSystem2(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen() => Observable.Never<Unit>();
        public void ForceRun() => ProcessBatch();
        public bool UseMultithreading(bool should) => ShouldMultithread = should;
            
        protected override void Process(Entity entity, ref StructComponent1Multiplexed component2, ref StructComponent3Multiplexed component3)
        {
            component2.Position += Vector3.One;
            component2.Something += 10;
            component3.IsTrue = true;
            component3.Value += 10;
        }
    }
    
    public class ClassBatchSystem3 : BatchedRefSystem<StructComponent2Multiplexed, StructComponent3Multiplexed>
    {
        public ClassBatchSystem3(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen() => Observable.Never<Unit>();
        public void ForceRun() => ProcessBatch();
        public bool UseMultithreading(bool should) => ShouldMultithread = should;
            
        protected override void Process(Entity entity, ref StructComponent2Multiplexed component2, ref StructComponent3Multiplexed component3)
        {
            component2.IsTrue = true;
            component2.Value += 10;
            component3.IsTrue = true;
            component3.Value += 10;
        }
    }
#endregion

#region Multiplex Systems
    public class Job1 : IMultiplexedRefJob<StructComponent1Multiplexed, StructComponent2Multiplexed, StructComponent3Multiplexed>
    {
        public void Process(Entity entity, ref StructComponent1Multiplexed component1, ref StructComponent2Multiplexed component2, ref StructComponent3Multiplexed component3)
        {
            component1.Position += Vector3.One;
            component1.Something += 10;
            component2.IsTrue = true;
            component2.Value += 10;
        }
    }
    
    public class Job2 : IMultiplexedRefJob<StructComponent1Multiplexed, StructComponent2Multiplexed, StructComponent3Multiplexed>
    {
        public void Process(Entity entity, ref StructComponent1Multiplexed component1, ref StructComponent2Multiplexed component2, ref StructComponent3Multiplexed component3)
        {
            component1.Position += Vector3.One;
            component1.Something += 10;
            component3.IsTrue = true;
            component3.Value += 10;
        }
    }
    
    public class Job3 : IMultiplexedRefJob<StructComponent1Multiplexed, StructComponent2Multiplexed, StructComponent3Multiplexed>
    {
        public void Process(Entity entity, ref StructComponent1Multiplexed component1, ref StructComponent2Multiplexed component2, ref StructComponent3Multiplexed component3)
        {
            component2.IsTrue = true;
            component2.Value += 10;
            component3.IsTrue = true;
            component3.Value += 10;
        }
    }

    public class ExampleMultiplexedSystem : MultiplexingBatchedRefSystem<StructComponent1Multiplexed, StructComponent2Multiplexed, StructComponent3Multiplexed>
    {
        public ExampleMultiplexedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen() => Observable.Never<Unit>();
        public void ForceRun() => ProcessBatch();
        public bool UseMultithreading(bool should) => ShouldMultithread = should;
        
        protected override IEnumerable<IMultiplexedRefJob<StructComponent1Multiplexed, StructComponent2Multiplexed, StructComponent3Multiplexed>> ResolveJobs()
        { return [new Job1(), new Job2(), new Job3()]; }
    }
    
#endregion

    [Params(10000)]
    public int EntityCount;

    [Params(false, true)]
    public bool UseMultithreading;
    
    [Params(1000)]
    public int Invocations;
    
    public ClassBatchSystem1 BatchingSystem1 { get; private set; }
    public ClassBatchSystem2 BatchingSystem2 { get; private set; }
    public ClassBatchSystem3 BatchingSystem3 { get; private set; }
    public ExampleMultiplexedSystem MultiplexedSystem { get; private set; }

    public override void Setup()
    {
        BatchingSystem1 = new ClassBatchSystem1(ComponentDatabase, EntityComponentAccessor, ComputedComponentGroupRegistry, new DefaultThreadHandler());
        BatchingSystem1.StartSystem();
        BatchingSystem2 = new ClassBatchSystem2(ComponentDatabase, EntityComponentAccessor, ComputedComponentGroupRegistry, new DefaultThreadHandler());
        BatchingSystem2.StartSystem();
        BatchingSystem3 = new ClassBatchSystem3(ComponentDatabase, EntityComponentAccessor, ComputedComponentGroupRegistry, new DefaultThreadHandler());
        BatchingSystem3.StartSystem();
        MultiplexedSystem = new ExampleMultiplexedSystem(ComponentDatabase, EntityComponentAccessor, ComputedComponentGroupRegistry, new DefaultThreadHandler());
        MultiplexedSystem.StartSystem();
        
        EntityCollection.CreateMany<BatchVsMultiplexedBlueprint>(EntityComponentAccessor, EntityCount);
    }

    public override void Cleanup()
    {
        EntityCollection.Clear();
        BatchingSystem1.StopSystem();
        BatchingSystem2.StopSystem();
        BatchingSystem3.StopSystem();
        MultiplexedSystem.StopSystem();
    }

    [Benchmark]
    public void ForceBatchRuns_Struct()
    {
        BatchingSystem1.UseMultithreading(UseMultithreading);
        BatchingSystem2.UseMultithreading(UseMultithreading);
        BatchingSystem3.UseMultithreading(UseMultithreading);
        for (var i = 0; i < Invocations; i++)
        {
            BatchingSystem1.ForceRun();
            BatchingSystem2.ForceRun();
            BatchingSystem3.ForceRun();
        }
    }
    
    [Benchmark]
    public void ForceMultiplexRuns_Struct()
    {
        MultiplexedSystem.UseMultithreading(UseMultithreading);
        for (var i = 0; i < Invocations; i++)
        { MultiplexedSystem.ForceRun(); }
    }
}