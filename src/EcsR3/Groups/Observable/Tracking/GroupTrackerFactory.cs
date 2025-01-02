using System.Collections.Generic;
using EcsR3.Collections.Entity;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Groups.Observable.Tracking.Trackers;
using EcsR3.Extensions;

namespace EcsR3.Groups.Observable.Tracking
{
    public class GroupTrackerFactory : IGroupTrackerFactory
    {
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public GroupTrackerFactory(IComponentTypeLookup componentTypeLookup)
        { ComponentTypeLookup = componentTypeLookup; }

        public ICollectionObservableGroupTracker TrackGroup(IGroup group, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingCollection> notifyingEntityComponentChanges)
        { return TrackGroup(ComponentTypeLookup.GetLookupGroupFor(group), initialEntities, notifyingEntityComponentChanges); }

        public ICollectionObservableGroupTracker TrackGroup(LookupGroup group, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingCollection> notifyingEntityComponentChanges)
        { return new CollectionObservableGroupTracker(group, initialEntities, notifyingEntityComponentChanges); }

        public IIndividualObservableGroupTracker TrackGroup(IGroup group, IEntity entity)
        { return TrackGroup(ComponentTypeLookup.GetLookupGroupFor(group), entity); }

        public IIndividualObservableGroupTracker TrackGroup(LookupGroup group, IEntity entity)
        { return new IndividualObservableGroupTracker(group, entity); }

        public IBatchObservableGroupTracker TrackGroup(IGroup group)
        { return TrackGroup(ComponentTypeLookup.GetLookupGroupFor(group)); }

        public IBatchObservableGroupTracker TrackGroup(LookupGroup group)
        { return new BatchObservableGroupTracker(group); }
    }
}