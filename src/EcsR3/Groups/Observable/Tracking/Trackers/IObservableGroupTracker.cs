using System;
using System.Collections.Generic;
using EcsR3.Groups.Observable.Tracking.Events;
using R3;

namespace EcsR3.Groups.Observable.Tracking.Trackers
{
    public interface IObservableGroupTracker : IDisposable
    {
        LookupGroup Group { get; }
        Observable<int> OnEntityJoinedGroup { get; }
        Observable<int> OnEntityLeavingGroup { get; }
        Observable<int> OnEntityLeftGroup { get; }
        
        IEnumerable<int> GetMatchedEntityIds();
    }
}