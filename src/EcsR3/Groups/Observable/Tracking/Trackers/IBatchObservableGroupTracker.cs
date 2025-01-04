using EcsR3.Entities;

namespace EcsR3.Groups.Observable.Tracking.Trackers
{
    public interface IBatchObservableGroupTracker : ICollectionObservableGroupTracker
    {
        bool StartTrackingEntity(IEntity entity);
        void StopTrackingEntity(IEntity entity);
    }
}