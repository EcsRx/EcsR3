using System.Collections.Generic;
using EcsR3.Collections.Entity;
using EcsR3.Entities;
using EcsR3.Groups.Observable.Tracking.Trackers;

namespace EcsR3.Groups.Observable.Tracking
{
    public interface IGroupTrackerFactory
    {
        ICollectionObservableGroupTracker TrackGroup(IGroup group, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingCollection> notifyingEntityComponentChanges);
        ICollectionObservableGroupTracker TrackGroup(LookupGroup group, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingCollection> notifyingEntityComponentChanges);
        IBatchObservableGroupTracker TrackGroup(IGroup group);
        IBatchObservableGroupTracker TrackGroup(LookupGroup group);
        IIndividualObservableGroupTracker TrackGroup(IGroup group, IEntity entity);
        IIndividualObservableGroupTracker TrackGroup(LookupGroup group, IEntity entity);
    }
}