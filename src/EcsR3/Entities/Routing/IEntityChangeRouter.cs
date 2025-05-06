using System;
using R3;

namespace EcsR3.Entities.Routing
{
    public interface IEntityChangeRouter : IDisposable
    {
        Observable<EntityChange> OnEntityAddedComponent(int componentType);
        Observable<EntityChange> OnEntityRemovingComponent(int componentType);
        Observable<EntityChange> OnEntityRemovedComponent(int componentType);
        
        void PublishEntityAddedComponent(int entityId, int componentId);
        void PublishEntityRemovingComponent(int entityId, int componentId);
        void PublishEntityRemovedComponent(int entityId, int componentId);
    }
}