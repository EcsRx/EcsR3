using System;
using EcsR3.Blueprints;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Components;

namespace EcsR3.Examples.ExampleApps.BatchedGroupExample.Blueprints
{
    public class MoveableBlueprint : IBlueprint
    {
        private const float MinimumMovementSpeed = 1;
        private const float MaximumMovementSpeed = 5;
        
        private readonly Random _random = new Random();

        public void Apply(IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            entityComponentAccessor.AddComponent(entityId, new NameComponent {Name = $"BatchedEntity-{entityId}"});
            entityComponentAccessor.CreateComponent<PositionComponent>(entityId);
                
            ref var movementSpeedComponent = ref entityComponentAccessor.CreateComponent<MovementSpeedComponent>(entityId);
            movementSpeedComponent.Speed = (float)_random.NextDouble() * (MaximumMovementSpeed - MinimumMovementSpeed) + MinimumMovementSpeed;
        }
    }
}