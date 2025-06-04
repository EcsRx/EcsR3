using System;

namespace EcsR3.Entities
{
    public readonly struct Entity : IEquatable<Entity>
    {
        public readonly int Id;
        public readonly int CreationHash;

        public Entity(int id, int creationHash)
        {
            Id = id;
            CreationHash = creationHash;
        }

        public bool Equals(Entity other)
        { return Id == other.Id && CreationHash == other.CreationHash; }

        public override bool Equals(object obj)
        { return obj is Entity other && Equals(other); }

        public override int GetHashCode()
        { return HashCode.Combine(Id, CreationHash); }

        public static bool operator ==(Entity left, Entity right)
        { return left.Equals(right); }

        public static bool operator !=(Entity left, Entity right)
        { return !left.Equals(right); }
    }
}