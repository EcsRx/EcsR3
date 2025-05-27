using System;
using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Extensions;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Components;

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
            entity.CreateComponent<PositionComponent>();
                
            ref var movementSpeedComponent = ref entity.CreateComponent<MovementSpeedComponent>();
            movementSpeedComponent.Speed = (float)_random.NextDouble() * (MaximumMovementSpeed - MinimumMovementSpeed) + MinimumMovementSpeed;
        }
    }
}