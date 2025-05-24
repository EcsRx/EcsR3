using System;
using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Examples.Custom.ComputedComponents.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.ComputedComponents.Blueprints;

public class ComputedStructComponentBlueprint : IBlueprint
{
    private Random _random = new Random();
    
    public void Apply(IEntity entity)
    {
        entity.AddComponents(new StructNumberComponent() { Value = _random.Next(0, 10)}, new StructNumber2Component() { Value = _random.Next(0, 10)});
    }
}