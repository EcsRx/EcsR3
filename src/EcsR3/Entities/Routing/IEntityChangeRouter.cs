using System;
using System.Collections.Generic;
using R3;

namespace EcsR3.Entities.Routing
{
    public interface IEntityChangeRouter : IDisposable
    {
        Observable<EntityChanges> OnEntityAddedComponents(int[] componentType);
        Observable<EntityChanges> OnEntityRemovingComponents(int[] componentType);
        Observable<EntityChanges> OnEntityRemovedComponents(int[] componentType);
        
        void PublishEntityAddedComponents(int entityId, int[] componentIds);
        void PublishEntityRemovingComponents(int entityId, int[] componentId);
        void PublishEntityRemovedComponents(int entityId, int[] componentId);
        
        void PublishEntityAddedComponents(int[] entityId, int[] componentIds);
        void PublishEntityRemovingComponents(int[] entityId, int[] componentId);
        void PublishEntityRemovedComponents(int[] entityId, int[] componentId);
    }
}