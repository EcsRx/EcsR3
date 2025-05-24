using System;
using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Examples.Custom.ComputedComponents.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.ComputedComponents.Blueprints;

public class ComputedComponentBlueprint : IBlueprint
{
    private Random _random = new Random();
    
    public void Apply(IEntity entity)
    {
        entity.AddComponents(new NumberComponent() { Value = _random.Next(0, 10)}, new Number2Component() { Value = _random.Next(0, 10)});
    }
}