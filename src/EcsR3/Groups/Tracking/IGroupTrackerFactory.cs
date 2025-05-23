using System.Collections.Generic;
using EcsR3.Entities;
using EcsR3.Groups.Tracking.Trackers;

namespace EcsR3.Groups.Tracking
{
    public interface IGroupTrackerFactory
    {
        IObservableGroupTracker TrackGroup(LookupGroup group, IEnumerable<IEntity> initialEntities = null);
    }
}