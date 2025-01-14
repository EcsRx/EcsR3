using System;
using EcsR3.Entities;
using EcsR3.Groups.Observable.Tracking.Types;

namespace EcsR3.Groups.Observable.Tracking.Events
{
    public readonly struct EntityGroupStateChanged : IEquatable<EntityGroupStateChanged>
    {
        public readonly IEntity Entity;
        public readonly GroupActionType GroupActionType;

        public EntityGroupStateChanged(IEntity entity, GroupActionType groupActionType)
        {
            Entity = entity;
            GroupActionType = groupActionType;
        }

        public bool Equals(EntityGroupStateChanged other)
        {
            return Equals(Entity, other.Entity) && GroupActionType == other.GroupActionType;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityGroupStateChanged other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Entity != null ? Entity.GetHashCode() : 0) * 397) ^ (int)GroupActionType;
            }
        }
    }
}