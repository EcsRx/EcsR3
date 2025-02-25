﻿using System;
using System.Collections.Generic;

namespace EcsR3.Collections.Events
{
    public readonly struct CollectionElementChangedEvent<T> : IEquatable<CollectionElementChangedEvent<T>>
    {
        public readonly int Index;
        public readonly T OldValue;
        public readonly T NewValue;

        public CollectionElementChangedEvent(int index, T oldValue, T newValue)
        {
            Index = index;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public bool Equals(CollectionElementChangedEvent<T> other)
        {
            return Index == other.Index && EqualityComparer<T>.Default.Equals(OldValue, other.OldValue) && EqualityComparer<T>.Default.Equals(NewValue, other.NewValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            return obj is CollectionElementChangedEvent<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Index;
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(OldValue);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(NewValue);
                return hashCode;
            }
        }
    }
}