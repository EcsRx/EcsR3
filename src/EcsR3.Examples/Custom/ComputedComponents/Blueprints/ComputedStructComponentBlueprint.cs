using System;
using EcsR3.Blueprints;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Custom.ComputedComponents.Components;

namespace EcsR3.Examples.Custom.ComputedComponents.Blueprints;

public class ComputedStructComponentBlueprint : IBlueprint, IBatchedBlueprint
{
    private Random _random = new Random();
    
    public void Apply(IEntityComponentAccessor entityComponentAccessor, int entityId)
    {
        ref var numberComponent = ref entityComponentAccessor.CreateComponent<StructNumberComponent>(entityId);
        numberComponent.Value = _random.Next(0, 10);
        
        ref var number2Component = ref entityComponentAccessor.CreateComponent<StructNumber2Component>(entityId);
        number2Component.Value = _random.Next(0, 10);
    }

    public void Apply(IEntityComponentAccessor entityComponentAccessor, int[] entityIds)
    {
        entityComponentAccessor.CreateComponents<StructNumberComponent, StructNumber2Component>(entityIds);

        for (var i = 0; i < entityIds.Length; i++)
        {
            var entityId = entityIds[i];
            ref var numberComponent = ref entityComponentAccessor.CreateComponent<StructNumberComponent>(entityId);
            numberComponent.Value = _random.Next(0, 10);
        
            ref var number2Component = ref entityComponentAccessor.CreateComponent<StructNumber2Component>(entityId);
            number2Component.Value = _random.Next(0, 10);
        }
    }
}