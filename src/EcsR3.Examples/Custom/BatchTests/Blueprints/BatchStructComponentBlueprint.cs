using System;
using System.Numerics;
using EcsR3.Blueprints;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Custom.BatchTests.Components;

namespace EcsR3.Examples.Custom.BatchTests.Blueprints;

public class BatchStructComponentBlueprint : IBlueprint, IBatchedBlueprint
{
    private Random _random = new Random();
    
    public void Apply(IEntityComponentAccessor entityComponentAccessor, int entityId)
    {
        ref var structComponent = ref entityComponentAccessor.CreateComponent<StructComponent>(entityId);
        structComponent.Something = _random.Next();
        structComponent.Position = new Vector3(_random.Next(), _random.Next(), _random.Next());
        
        ref var struct2Component = ref entityComponentAccessor.CreateComponent<StructComponent2>(entityId);
        struct2Component.Value = _random.Next(0, 10);
    }

    public void Apply(IEntityComponentAccessor entityComponentAccessor, int[] entityIds)
    {
        entityComponentAccessor.CreateComponents<StructComponent, StructComponent2>(entityIds);

        for (var i = 0; i < entityIds.Length; i++)
        {
            var entityId = entityIds[i];
            ref var structComponent = ref entityComponentAccessor.GetComponentRef<StructComponent>(entityId);
            structComponent.Something = _random.Next();
            structComponent.Position = new Vector3(_random.Next(), _random.Next(), _random.Next());
        
            ref var struct2Component = ref entityComponentAccessor.GetComponentRef<StructComponent2>(entityId);
            struct2Component.Value = _random.Next(0, 10);
        }
    }
}