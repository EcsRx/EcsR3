using EcsR3.Groups.Observable.Tracking.Types;

namespace EcsR3.Groups.Observable.Tracking.Events
{
    public readonly struct EntityGroupStateChanged
    {
        public readonly int EntityId;
        public readonly GroupActionType GroupActionType;

        public EntityGroupStateChanged(int entityId, GroupActionType groupActionType)
        {
            EntityId = entityId;
            GroupActionType = groupActionType;
        }
    }
}