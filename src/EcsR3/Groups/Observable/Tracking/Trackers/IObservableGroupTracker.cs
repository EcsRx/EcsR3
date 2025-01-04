using System;
using EcsR3.Groups.Observable.Tracking.Events;
using R3;

namespace EcsR3.Groups.Observable.Tracking.Trackers
{
    public interface IObservableGroupTracker : IDisposable
    {
        Observable<EntityGroupStateChanged> GroupMatchingChanged { get; }
    }
}