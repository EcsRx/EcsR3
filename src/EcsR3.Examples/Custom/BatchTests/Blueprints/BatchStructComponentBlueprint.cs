using System;
using System.Numerics;
using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Examples.Custom.BatchTests.Components;

namespace EcsR3.Examples.Custom.BatchTests.Blueprints;

public class BatchStructComponentBlueprint : IBlueprint
{
    private Random _random = new Random();
    
    public void Apply(IEntity entity)
    {
        ref var structComponent = ref entity.CreateComponent<StructComponent>();
        structComponent.Something = _random.Next();
        structComponent.Position = new Vector3(_random.Next(), _random.Next(), _random.Next());
        
        ref var struct2Component = ref entity.CreateComponent<StructComponent2>();
        struct2Component.Value = _random.Next(0, 10);
    }
}