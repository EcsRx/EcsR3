using System.Threading;
using EcsR3.Blueprints;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Extensions;
using EcsR3.Systems.Batching.Convention;
using EcsR3.Tests.Models;
using R3;
using SystemsR3.Attributes;
using SystemsR3.Threading;

namespace EcsR3.Tests.Systems;

public class BatchedProcessTestSystem : BatchedSystem<TestComponentOne, TestComponentTwo>
{
    public class BatchedProcessTestBlueprint : IBlueprint
    {
        public void Apply(IEntity entity)
        {
            entity.AddComponents(new TestComponentOne(), new TestComponentTwo());
        }
    }
    
    public BatchedProcessTestSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
    {
    }
    
    public void ForceProcess() => ProcessBatch();
    public long TimesCalled = 0;

    protected override Observable<bool> ReactWhen() => Observable.Never<bool>();

    protected override void Process(int entityId, TestComponentOne component1, TestComponentTwo component2)
    {
        Interlocked.Add(ref TimesCalled, 1);
    }
}

[MultiThread]
public class MultithreadedBatchedProcessTestSystem : BatchedProcessTestSystem
{
    public MultithreadedBatchedProcessTestSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
    {
    }
}