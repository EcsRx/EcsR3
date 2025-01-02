using System;
using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Extensions;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Components;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Lookups;

namespace EcsR3.Examples.ExampleApps.BatchedGroupExample.Blueprints
{
    public class MoveableBlueprint : IBlueprint
    {
        private const float MinimumMovementSpeed = 1;
        private const float MaximumMovementSpeed = 5;
        
        private readonly Random _random = new Random();

        public void Apply(IEntity entity)
        {
            entity.AddComponent(new NameComponent {Name = $"BatchedEntity-{entity.Id}"});
            entity.AddComponent<PositionComponent>(ComponentLookupTypes.PositionComponentId);
                
            ref var movementSpeedComponent = ref entity.AddComponent<MovementSpeedComponent>(ComponentLookupTypes.MovementSpeedComponentId);
            movementSpeedComponent.Speed = (float)_random.NextDouble() * (MaximumMovementSpeed - MinimumMovementSpeed) + MinimumMovementSpeed;
        }
    }
}