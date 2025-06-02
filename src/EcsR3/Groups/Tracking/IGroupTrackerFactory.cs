using System.Collections.Generic;
using EcsR3.Entities;
using EcsR3.Groups.Tracking.Trackers;

namespace EcsR3.Groups.Tracking
{
    public interface IGroupTrackerFactory
    {
        IComputedEntityGroupTracker TrackGroup(LookupGroup group, IEnumerable<Entity> initialEntities = null);
    }
}