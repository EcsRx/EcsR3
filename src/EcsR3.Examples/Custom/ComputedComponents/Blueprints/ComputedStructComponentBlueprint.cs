using System;
using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Examples.Custom.ComputedComponents.Components;

namespace EcsR3.Examples.Custom.ComputedComponents.Blueprints;

public class ComputedStructComponentBlueprint : IBlueprint
{
    private Random _random = new Random();
    
    public void Apply(IEntity entity)
    {
        ref var numberComponent = ref entity.CreateComponent<StructNumberComponent>();
        numberComponent.Value = _random.Next(0, 10);
        
        ref var number2Component = ref entity.CreateComponent<StructNumber2Component>();
        number2Component.Value = _random.Next(0, 10);
    }
}