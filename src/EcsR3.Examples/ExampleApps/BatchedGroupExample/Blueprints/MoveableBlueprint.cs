using System;
using EcsR3.Blueprints;
using EcsR3.Entities;
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

        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity[] entities)
        {
            entityComponentAccessor.CreateComponents<PositionComponent, MovementSpeedComponent, NameComponent>(entities);
            var nameComponents = entityComponentAccessor.GetComponent<NameComponent>(entities);
            for (var i = 0; i < entities.Length; i++)
            {
                var entityId = entities[i];
                var nameComponent = nameComponents[i];
                nameComponent.Name = $"BatchedEntity-{entityId}";
                
                ref var movementSpeed = ref entityComponentAccessor.GetComponentRef<MovementSpeedComponent>(entityId);
                movementSpeed.Speed = (float)_random.NextDouble() * (MaximumMovementSpeed - MinimumMovementSpeed) + MinimumMovementSpeed;
            }
        }

        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            entityComponentAccessor.CreateComponents<NameComponent, PositionComponent, MovementSpeedComponent>([entity]);
            
            entityComponentAccessor.GetComponent<NameComponent>(entity).Name = $"Entity-{entity}";
            ref var movementSpeed = ref entityComponentAccessor.GetComponentRef<MovementSpeedComponent>(entity);
            movementSpeed.Speed = (float)_random.NextDouble() * (MaximumMovementSpeed - MinimumMovementSpeed) + MinimumMovementSpeed;
        }
    }
}