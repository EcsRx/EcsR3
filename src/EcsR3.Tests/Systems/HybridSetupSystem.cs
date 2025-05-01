using System;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Systems;
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

    public void Setup(IEntity entity)
    {
        OnMethodCalled("setup");
    }
    
    public void Teardown(IEntity entity)
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