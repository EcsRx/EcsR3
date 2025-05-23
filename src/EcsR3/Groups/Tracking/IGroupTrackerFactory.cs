using System.Collections.Generic;
using EcsR3.Entities;
using EcsR3.Groups.Observable.Tracking.Trackers;

namespace EcsR3.Groups.Observable.Tracking
{
    public interface IGroupTrackerFactory
    {
        IObservableGroupTracker TrackGroup(LookupGroup group, IEnumerable<IEntity> initialEntities = null);
    }
}