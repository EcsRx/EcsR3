using System;
using System.Collections.Generic;
using EcsR3.Entities;
using R3;

namespace EcsR3.Groups.Tracking.Trackers
{
    /// <summary>
    /// Tracks changes to a given group and provides events for when entities join/leave the group
    /// </summary>
    public interface IComputedEntityGroupTracker : IDisposable
    {
        /// <summary>
        /// The group that is being tracked
        /// </summary>
        LookupGroup Group { get; }
        
        /// <summary>
        /// Provides the entity Id that has been added to the group
        /// </summary>
        Observable<Entity> OnEntityJoinedGroup { get; }
        
        /// <summary>
        /// Provides the entity Id that is leaving the group
        /// </summary>
        Observable<Entity> OnEntityLeavingGroup { get; }
        
        /// <summary>
        /// Provides the entity if that has left the group
        /// </summary>
        Observable<Entity> OnEntityLeftGroup { get; }
        
        /// <summary>
        /// Gets all entity ids that are currently in the group
        /// </summary>
        /// <returns></returns>
        IEnumerable<Entity> GetMatchedEntities();
    }
}