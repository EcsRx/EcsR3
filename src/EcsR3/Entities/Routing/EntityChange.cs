using System;

namespace EcsR3.Entities.Routing
{
    /// <summary>
    /// The component changes that have occurred on an entity
    /// </summary>
    public readonly struct EntityChanges
    {
        /// <summary>
        /// The id of the entity that has changed
        /// </summary>
        public readonly Entity Entity;
        
        /// <summary>
        /// The component ids that have changed
        /// </summary>
        /// <remarks>This is a memory to reduce allocations</remarks>
        public readonly ReadOnlyMemory<int> ComponentIds;

        public EntityChanges(Entity entity, ReadOnlyMemory<int> componentIds)
        {
            Entity = entity;
            ComponentIds = componentIds;
        }
    }
}