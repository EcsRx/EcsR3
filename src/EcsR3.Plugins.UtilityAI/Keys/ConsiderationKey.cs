using System;

namespace EcsR3.Plugins.UtilityAI.Keys
{
    public readonly struct ConsiderationKey : IEquatable<ConsiderationKey>
    {
        public const int NoRelatedIds = -1;
        
        public readonly int ConsiderationId;
        public readonly int RelatedId;

        public ConsiderationKey(int considerationId, int relatedId = NoRelatedIds)
        {
            ConsiderationId = considerationId;
            RelatedId = relatedId;
        }

        public bool Equals(ConsiderationKey other)
        {
            return ConsiderationId == other.ConsiderationId && RelatedId == other.RelatedId;
        }

        public override bool Equals(object obj)
        {
            return obj is ConsiderationKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ConsiderationId * 397) ^ RelatedId;
            }
        }
    }
}