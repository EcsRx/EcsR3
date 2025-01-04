namespace EcsR3.Groups.Observable.Tracking.Trackers
{
    public interface ICollectionObservableGroupTracker : IObservableGroupTracker
    {
        bool IsMatching(int entityId);
    }
}