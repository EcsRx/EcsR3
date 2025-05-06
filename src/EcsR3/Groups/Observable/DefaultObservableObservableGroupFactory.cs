using EcsR3.Collections.Entity;
using EcsR3.Groups.Observable.Tracking;

namespace EcsR3.Groups.Observable
{
    public class DefaultObservableObservableGroupFactory : IObservableGroupFactory
    {
        public IGroupTrackerFactory GroupTrackerFactory { get; }
        public IEntityCollection EntityCollection { get; }
        
        public DefaultObservableObservableGroupFactory(IGroupTrackerFactory groupTrackerFactory, IEntityCollection entityCollection)
        {
            GroupTrackerFactory = groupTrackerFactory;
            EntityCollection = entityCollection;
        }

        public IObservableGroup Create(ObservableGroupConfiguration arg)
        {
            var tracker = GroupTrackerFactory.TrackGroup(arg.Group, EntityCollection);
            return new ObservableGroup(arg.Group, tracker, EntityCollection);
        }
    }
}