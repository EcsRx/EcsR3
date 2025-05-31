using System;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Reactive;
using SystemsR3.Systems.Conventional;

namespace EcsR3.Tests.Systems;

public class HybridSetupSystem : ISetupSystem, ITeardownSystem, IManualSystem
{
    public IGroup Group { get; }

    public Action<string> OnMethodCalled { get; }

    public HybridSetupSystem(Action<string> onMethodCalled, IGroup group)
    {
        Group = group;
        OnMethodCalled = onMethodCalled;
    }

    public void Setup(IEntityComponentAccessor entityComponentAccessor, int entityId)
    {
        OnMethodCalled("setup");
    }
    
    public void Teardown(IEntityComponentAccessor entityComponentAccessor, int entityId)
    {
        OnMethodCalled("teardown");
    }
    
    public void StartSystem()
    {
        OnMethodCalled("start-system");
    }

    public void StopSystem()
    {
        OnMethodCalled("stop-system");
    }
}