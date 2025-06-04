using System;
using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Custom.ComputedComponents.Components;

namespace EcsR3.Examples.Custom.ComputedComponents.Blueprints;

public class ComputedStructComponentBlueprint : IBlueprint, IBatchedBlueprint
{
    private Random _random = new Random();
    
    public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
    {
        ref var numberComponent = ref entityComponentAccessor.CreateComponent<StructNumberComponent>(entity);
        numberComponent.Value = _random.Next(0, 10);
        
        ref var number2Component = ref entityComponentAccessor.CreateComponent<StructNumber2Component>(entity);
        number2Component.Value = _random.Next(0, 10);
    }

    public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity[] entities)
    {
        entityComponentAccessor.CreateComponents<StructNumberComponent, StructNumber2Component>(entities);

        for (var i = 0; i < entities.Length; i++)
        {
            var entityId = entities[i];
            ref var numberComponent = ref entityComponentAccessor.CreateComponent<StructNumberComponent>(entityId);
            numberComponent.Value = _random.Next(0, 10);
        
            ref var number2Component = ref entityComponentAccessor.CreateComponent<StructNumber2Component>(entityId);
            number2Component.Value = _random.Next(0, 10);
        }
    }
}