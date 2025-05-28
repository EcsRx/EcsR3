using System;
using EcsR3.Blueprints;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Components;

namespace EcsR3.Examples.ExampleApps.BatchedGroupExample.Blueprints
{
    public class MoveableBlueprint : IBlueprint, IBatchedBlueprint
    {
        private const float MinimumMovementSpeed = 1;
        private const float MaximumMovementSpeed = 5;
        
        private readonly Random _random = new Random();

        public void Apply(IEntityComponentAccessor entityComponentAccessor, int[] entityIds)
        {
            entityComponentAccessor.CreateComponents<PositionComponent, MovementSpeedComponent, NameComponent>(entityIds);
            var nameComponents = entityComponentAccessor.GetComponent<NameComponent>(entityIds);
            for (var i = 0; i < entityIds.Length; i++)
            {
                var entityId = entityIds[i];
                var nameComponent = nameComponents[i];
                nameComponent.Name = $"BatchedEntity-{entityId}";
                
                ref var movementSpeed = ref entityComponentAccessor.GetComponentRef<MovementSpeedComponent>(entityId);
                movementSpeed.Speed = (float)_random.NextDouble() * (MaximumMovementSpeed - MinimumMovementSpeed) + MinimumMovementSpeed;
            }
        }

        public void Apply(IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            entityComponentAccessor.CreateComponents<NameComponent, PositionComponent, MovementSpeedComponent>([entityId]);
            
            entityComponentAccessor.GetComponent<NameComponent>(entityId).Name = $"Entity-{entityId}";
            ref var movementSpeed = ref entityComponentAccessor.GetComponentRef<MovementSpeedComponent>(entityId);
            movementSpeed.Speed = (float)_random.NextDouble() * (MaximumMovementSpeed - MinimumMovementSpeed) + MinimumMovementSpeed;
        }
    }
}