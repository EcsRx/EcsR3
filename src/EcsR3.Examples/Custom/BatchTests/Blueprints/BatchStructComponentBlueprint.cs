using System;
using System.Numerics;
using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Custom.BatchTests.Components;

namespace EcsR3.Examples.Custom.BatchTests.Blueprints;

public class BatchStructComponentBlueprint : IBlueprint, IBatchedBlueprint
{
    private Random _random = new Random();
    
    public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
    {
        ref var structComponent = ref entityComponentAccessor.CreateComponent<StructComponent>(entity);
        structComponent.Something = _random.Next();
        structComponent.Position = new Vector3(_random.Next(), _random.Next(), _random.Next());
        
        ref var struct2Component = ref entityComponentAccessor.CreateComponent<StructComponent2>(entity);
        struct2Component.Value = _random.Next(0, 10);
    }

    public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity[] entities)
    {
        entityComponentAccessor.CreateComponents<StructComponent, StructComponent2>(entities);
        var structComponents = entityComponentAccessor.GetComponentRef<StructComponent>(entities);
        for (var i = 0; i < structComponents.Count; i++)
        {
            ref var structComponent = ref structComponents[i];
            structComponent.Something = _random.Next();
            structComponent.Position = new Vector3(_random.Next(), _random.Next(), _random.Next());
        }
        
        var structComponent2 = entityComponentAccessor.GetComponentRef<StructComponent2>(entities);
        for (var i = 0; i < structComponent2.Count; i++)
        {
            ref var struct2Component = ref structComponent2[i];
            struct2Component.Value = _random.Next(0, 10);
        }
    }
}