using System;
using R3;

namespace EcsR3.Entities.Routing
{
    public interface IEntityChangeRouter : IDisposable
    {
        Observable<EntityChange> SubscribeOnEntityAddedComponent(int componentType);
        Observable<EntityChange> SubscribeOnEntityRemovingComponent(int componentType);
        Observable<EntityChange> SubscribeOnEntityRemovedComponent(int componentType);
        
        void PublishEntityAddedComponent(int entityId, int componentId);
        void PublishEntityRemovingComponent(int entityId, int componentId);
        void PublishEntityRemovedComponent(int entityId, int componentId);
    }
}