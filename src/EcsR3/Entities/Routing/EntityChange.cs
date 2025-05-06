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
}