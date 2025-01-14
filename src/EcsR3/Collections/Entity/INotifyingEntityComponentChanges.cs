using System;
using EcsR3.Collections.Events;
using R3;

namespace EcsR3.Collections.Entity
{
    public interface INotifyingEntityComponentChanges
    {
        Observable<ComponentsChangedEvent> EntityComponentsAdded { get; }
        Observable<ComponentsChangedEvent> EntityComponentsRemoving { get; }
        Observable<ComponentsChangedEvent> EntityComponentsRemoved { get; }
    }
}