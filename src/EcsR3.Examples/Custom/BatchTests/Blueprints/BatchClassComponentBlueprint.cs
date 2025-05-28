using System;
using System.Numerics;
using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Custom.BatchTests.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.BatchTests.Blueprints;

public class BatchClassComponentBlueprint : IBlueprint, IBatchedBlueprint
{
    private Random _random = new Random();
    
    public void Apply(IEntityComponentAccessor entityComponentAccessor, int entityId)
    {
        entityComponentAccessor.AddComponents(entityId, new ClassComponent()
            {
                Position = new Vector3(_random.Next(), _random.Next(), _random.Next()),
                Something = _random.Next(0, 1)
            }, 
            new ClassComponent2() { Value = _random.Next(0, 10)});
    }

    public void Apply(IEntityComponentAccessor entityComponentAccessor, int[] entityIds)
    {
        entityComponentAccessor.CreateComponents<ClassComponent, ClassComponent2>(entityIds);
        var classComponents = entityComponentAccessor.GetComponent<ClassComponent>(entityIds);
        foreach (var classComponent in classComponents)
        {
            classComponent.Position = new Vector3(_random.Next(), _random.Next(), _random.Next());
            classComponent.Something = _random.Next(0, 1);
        }
        
        var classComponent2s = entityComponentAccessor.GetComponent<ClassComponent2>(entityIds);
        foreach (var classComponent2 in classComponent2s)
        { classComponent2.Value = _random.Next(0, 10); }
    }
}