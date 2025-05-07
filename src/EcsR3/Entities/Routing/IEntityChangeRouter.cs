using System;
using R3;

namespace EcsR3.Entities.Routing
{
    public interface IEntityChangeRouter : IDisposable
    {
        Observable<EntityChange> OnEntityAddedComponent(int componentType);
        Observable<EntityChange> OnEntityAddedComponent(params int[] componentType);
        Observable<EntityChanges> OnEntityAddedComponents(params int[] componentTypes);
        Observable<EntityChange> OnEntityRemovingComponent(int componentType);
        Observable<EntityChange> OnEntityRemovedComponent(int componentType);
        
        void PublishEntityAddedComponent(int entityId, int componentId);
        void PublishEntityAddedComponents(int entityId, int[] componentIds);
        void PublishEntityRemovingComponent(int entityId, int componentId);
        void PublishEntityRemovedComponent(int entityId, int componentId);
    }
}