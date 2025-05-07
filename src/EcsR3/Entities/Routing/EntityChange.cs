using System;

namespace EcsR3.Entities.Routing
{
    public readonly struct EntityChange
    {
        public readonly int EntityId;
        public readonly int ComponentId;

        public EntityChange(int entityId, int componentId)
        {
            EntityId = entityId;
            ComponentId = componentId;
        }
    }

    public readonly struct EntityChanges
    {
        public readonly int EntityId;
        public readonly ReadOnlyMemory<int> ComponentIds;

        public EntityChanges(int entityId, ReadOnlyMemory<int> componentIds)
        {
            EntityId = entityId;
            ComponentIds = componentIds;
        }
    }
}