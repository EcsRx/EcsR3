namespace EcsR3.Groups.Observable.Tracking.Trackers
{
    public interface IIndividualObservableGroupTracker : IObservableGroupTracker
    {
        bool IsMatching();
    }
}