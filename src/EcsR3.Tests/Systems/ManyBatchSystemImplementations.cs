using System;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Batching.Convention;
using EcsR3.Systems.Reactive;
using EcsR3.Tests.Models;
using R3;
using SystemsR3.Systems.Conventional;
using SystemsR3.Threading;

namespace EcsR3.Tests.Systems;

public class ManyBatchSystemImplementations : BatchedSystem<TestComponentOne, TestComponentTwo>, ISetupSystem, ITeardownSystem
{
    public ManyBatchSystemImplementations(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
    {
    }

    public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
    { }
    
    public void Teardown(IEntityComponentAccessor entityComponentAccessor, Entity entity)
    { }

    protected override Observable<Unit> ReactWhen()
    { return Observable.Never<Unit>(); }

    public void StartSystem()
    { }

    public void StopSystem()
    { }

    protected override void Process(Entity entity, TestComponentOne component1, TestComponentTwo component2)
    { }
}