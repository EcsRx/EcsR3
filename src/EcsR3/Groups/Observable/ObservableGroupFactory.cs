﻿using EcsR3.Collections.Entity;
using EcsR3.Groups.Observable.Tracking;

namespace EcsR3.Groups.Observable
{
    public class ObservableGroupFactory : IObservableGroupFactory
    {
        public IGroupTrackerFactory GroupTrackerFactory { get; }
        public IEntityCollection EntityCollection { get; }
        
        public ObservableGroupFactory(IGroupTrackerFactory groupTrackerFactory, IEntityCollection entityCollection)
        {
            GroupTrackerFactory = groupTrackerFactory;
            EntityCollection = entityCollection;
        }

        public IObservableGroup Create(LookupGroup group)
        {
            var tracker = GroupTrackerFactory.TrackGroup(group, EntityCollection);
            return new ObservableGroup(group, tracker, EntityCollection);
        }
    }
}