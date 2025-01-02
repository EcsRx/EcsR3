using System;
using EcsR3.Collections.Events;
using R3;

namespace EcsR3.Collections.Entity
{
    public interface INotifyingEntityCollection
    {
        Observable<CollectionEntityEvent> EntityAdded { get; }
        Observable<CollectionEntityEvent> EntityRemoved { get; }
    }
}