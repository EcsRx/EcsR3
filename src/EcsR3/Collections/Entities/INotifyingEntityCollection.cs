using EcsR3.Collections.Events;
using R3;

namespace EcsR3.Collections.Entities
{
    public interface INotifyingEntityCollection
    {
        Observable<CollectionEntityEvent> EntityAdded { get; }
        Observable<CollectionEntityEvent> EntityRemoved { get; }
    }
}