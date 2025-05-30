using System;
using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Custom.ComputedComponents.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.ComputedComponents.Blueprints;

public class ComputedComponentBlueprint : IBlueprint, IBatchedBlueprint
{
    private Random _random = new Random();
    
    public void Apply(IEntityComponentAccessor entityComponentAccessor, int entityId)
    {
        entityComponentAccessor.AddComponents(entityId, new NumberComponent() { Value = _random.Next(0, 10)}, new Number2Component() { Value = _random.Next(0, 10)});
    }

    public void Apply(IEntityComponentAccessor entityComponentAccessor, int[] entityIds)
    {
        entityComponentAccessor.CreateComponents<NumberComponent, Number2Component>(entityIds);
        
        var numberComponents = entityComponentAccessor.GetComponent<NumberComponent>(entityIds);
        foreach (var numberComponent in numberComponents) { numberComponent.Value = _random.Next(0, 10); }
        
        var number2Components = entityComponentAccessor.GetComponent<Number2Component>(entityIds);
        foreach (var number2Component in number2Components) { number2Component.Value = _random.Next(0, 10); }
    }
}