using System;
using System.Collections.Generic;
using EcsR3.Collections.Entity;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities.Routing;
using EcsR3.Extensions;

namespace EcsR3.Entities.Accessors
{
    public class EntityComponentAccessor : IEntityComponentAccessor
    {
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IEntityAllocationDatabase EntityAllocationDatabase { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IEntityChangeRouter EntityChangeRouter { get; }

        public EntityComponentAccessor(IComponentTypeLookup componentTypeLookup, IEntityAllocationDatabase entityAllocationDatabase, IComponentDatabase componentDatabase, IEntityChangeRouter entityChangeRouter)
        {
            ComponentTypeLookup = componentTypeLookup;
            EntityAllocationDatabase = entityAllocationDatabase;
            ComponentDatabase = componentDatabase;
            EntityChangeRouter = entityChangeRouter;
        }
        
        public void AddComponents(int entityId, IReadOnlyList<IComponent> components)
        {
            var componentTypeIds = new int[components.Count];
            for (var i = 0; i < components.Count; i++)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentTypeId(components[i].GetType());
                var allocationId = EntityAllocationDatabase.AllocateComponent(componentTypeId, entityId);
                ComponentDatabase.Set(componentTypeId, allocationId, components[i]);
                componentTypeIds[i] = componentTypeId;
            }
            
            EntityChangeRouter.PublishEntityAddedComponents(entityId, componentTypeIds);
        }
        
        public ref T CreateComponent<T>(int entityId) where T : struct, IComponent
        {
            var defaultComponent = new T();
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(typeof(T));
            var allocationId = EntityAllocationDatabase.AllocateComponent(componentTypeId, entityId);
            ComponentDatabase.Set(componentTypeId, allocationId, defaultComponent);
            return ref ComponentDatabase.GetRef<T>(componentTypeId, allocationId);
        }
        
        public void RemoveComponents(int entityId, IReadOnlyList<int> componentsTypeIds)
        {
            var lastIndex = 0;
            Span<int> temporaryComponentIds = stackalloc int[componentsTypeIds.Count];
            for (var i = 0; i < componentsTypeIds.Count; i++)
            {
                var componentTypeId = componentsTypeIds[i];
                if(EntityAllocationDatabase.HasComponent(componentTypeId, entityId))
                { temporaryComponentIds[lastIndex++] = componentsTypeIds[i]; }
            }
            if(temporaryComponentIds.Length == 0) { return; }
            var sanitisedComponentsIds = temporaryComponentIds[..lastIndex].ToArray();
            
            EntityChangeRouter.PublishEntityRemovingComponents(entityId, sanitisedComponentsIds);
 
            for (var i = 0; i < sanitisedComponentsIds.Length; i++)
            {
                var componentId = sanitisedComponentsIds[i];
                var allocationIndex = EntityAllocationDatabase.ReleaseComponent(componentId, entityId);
                ComponentDatabase.Remove(componentId, allocationIndex);
            }
            
            EntityChangeRouter.PublishEntityRemovedComponents(entityId, sanitisedComponentsIds);
        }

        public void RemoveComponents(int entityId, params Type[] componentTypes)
        {
            var componentTypeIds = ComponentTypeLookup.GetComponentTypeIds(componentTypes);
            RemoveComponents(entityId, componentTypeIds);
        }

        public void RemoveAllComponents(int entityId)
        {
            var allComponentIds = EntityAllocationDatabase.GetAllEntityComponents(entityId);
            RemoveComponents(entityId, allComponentIds);
        }

        public IComponent GetComponent(int entityId, Type componentType)
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(componentType);
            var allocationId = EntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, entityId);
            return ComponentDatabase.Get(allocationId, componentTypeId);
        }

        public ref T GetComponentRef<T>(int entityId) where T : struct, IComponent
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(typeof(T));
            var allocationId = EntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, entityId);
            return ref ComponentDatabase.GetRef<T>(allocationId, componentTypeId);
        }
        
        public void UpdateComponent<T>(int entityId, T newValue) where T : struct, IComponent
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(typeof(T));
            var allocationId = EntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, entityId);;
            ComponentDatabase.Set(componentTypeId, allocationId, newValue);
        }

        public bool HasComponent(int entityId, Type componentType)
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(componentType);
            return EntityAllocationDatabase.HasComponent(componentTypeId, entityId);
        }
    }
}