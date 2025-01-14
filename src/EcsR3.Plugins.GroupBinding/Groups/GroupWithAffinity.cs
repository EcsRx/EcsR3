﻿using EcsR3.Groups;

namespace EcsR3.Plugins.GroupBinding.Groups
{
    public struct GroupWithAffinity
    {
        public static GroupWithAffinity Default { get; } = new GroupWithAffinity();

        public IGroup Group { get; }
        public int[] CollectionIds { get; }

        public GroupWithAffinity(IGroup group, int[] collectionIds)
        {
            Group = group;
            CollectionIds = collectionIds;
        }
    }
}