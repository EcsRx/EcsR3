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
        
        void PublishEntityAddedComponents(Entity entity, int[] componentIds);
        void PublishEntityRemovingComponents(Entity entity, int[] componentId);
        void PublishEntityRemovedComponents(Entity entity, int[] componentId);
        
        void PublishEntityAddedComponents(Entity[] entities, int[] componentIds);
        void PublishEntityRemovingComponents(Entity[] entities, int[] componentId);
        void PublishEntityRemovedComponents(Entity[] entities, int[] componentId);
    }
}