using System;
using System.Collections.Generic;
using EcsR3.Groups;

namespace EcsR3.Computeds.Entities.Registries
{
    public interface IComputedEntityGroupRegistry : IDisposable
    {
        IEnumerable<IComputedEntityGroup> ComputedGroups { get; }
        IEnumerable<IComputedEntityGroup> GetApplicableGroups(int[] componentTypeIds);
        
        /// <summary>
        /// Gets an ComputedEntityroup which will observe the given group and maintain a collection of
        /// entities which are applicable. This is the preferred way to access entities inside collections.
        /// </summary>
        /// <remarks>
        /// It is worth noting that IComputedEntityGroup instances are cached within the manager, so if there is
        /// a request for an Computed Entity group targetting the same underlying components (not the IGroup instance, but
        /// the actual components the group cares about) it will return the existing group, if one does not exist
        /// it is created.
        /// </remarks>
        /// <param name="group">The group to match entities on</param>
        /// <returns>An IComputedEntityGroup monitoring the group passed in</returns>
        IComputedEntityGroup GetComputedGroup(IGroup group);
        
        IComputedEntityGroup GetComputedGroup(LookupGroup group);
    }
}