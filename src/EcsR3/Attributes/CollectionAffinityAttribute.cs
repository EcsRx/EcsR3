using System;
using EcsR3.Collections.Database;

namespace EcsR3.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class CollectionAffinityAttribute : Attribute
    {
        public int[] CollectionIds { get; }

        public CollectionAffinityAttribute(int collectionId = EntityCollectionLookups.DefaultCollectionId)
        { CollectionIds = new []{collectionId}; }

        public CollectionAffinityAttribute(params int[] collectionIds)
        { CollectionIds = collectionIds; }
    }
}