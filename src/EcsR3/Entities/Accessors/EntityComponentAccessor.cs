using System;
using System.Collections.Generic;
using EcsR3.Collections.Entities;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities.Routing;
using SystemsR3.Utility;

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
        
        public void AddComponents(Entity entity, IReadOnlyList<IComponent> components)
        {
            var componentTypeIds = ComponentTypeLookup.GetComponentTypeIds(components);
            var allocationIds = EntityAllocationDatabase.AllocateComponents(componentTypeIds, entity);
            ComponentDatabase.SetMany(componentTypeIds, allocationIds, components);
            
            EntityChangeRouter.PublishEntityAddedComponents(entity, componentTypeIds);
        }
        
        public ref T CreateComponent<T>(Entity entity) where T : struct, IComponent
        {
            var defaultComponent = new T();
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(typeof(T));
            var allocationId = EntityAllocationDatabase.AllocateComponent(componentTypeId, entity);
            ComponentDatabase.Set(componentTypeId, allocationId, defaultComponent);
            return ref ComponentDatabase.GetRef<T>(componentTypeId, allocationId);
        }

        protected int CreateComponentBatch<T>(Entity[] entities) where T : IComponent, new()
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(typeof(T));
            var allocationIds = EntityAllocationDatabase.AllocateComponent(componentTypeId, entities);
            
            var isReferenceType = !typeof(T).IsValueType;
            var newComponents = new T[entities.Length];
            if (isReferenceType)
            {
                for (var i = 0; i < entities.Length; i++)
                { newComponents[i] = new T(); }
            }
            ComponentDatabase.Set(componentTypeId, allocationIds, newComponents);
            return componentTypeId;
        }
        
        public void CreateComponent<T>(Entity[] entities) where T : IComponent, new()
        {
            var componentTypeIds = new[]
            {
                CreateComponentBatch<T>(entities)
            };
            
            EntityChangeRouter.PublishEntityAddedComponents(entities, componentTypeIds);
        }
        
        public void CreateComponents<T1, T2>(Entity[] entities) where T1 : IComponent, new() where T2 : IComponent, new()
        {
            var componentTypeIds = new[]
            {
                CreateComponentBatch<T1>(entities),
                CreateComponentBatch<T2>(entities)
            };
            
            EntityChangeRouter.PublishEntityAddedComponents(entities, componentTypeIds);
        }

        public void CreateComponents<T1, T2, T3>(Entity[] entities) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new()
        {
            var componentTypeIds = new[]
            {
                CreateComponentBatch<T1>(entities),
                CreateComponentBatch<T2>(entities),
                CreateComponentBatch<T3>(entities)
            };
            
            EntityChangeRouter.PublishEntityAddedComponents(entities, componentTypeIds);
        }

        public void CreateComponents<T1, T2, T3, T4>(Entity[] entities) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new()
        {
            var componentTypeIds = new[]
            {
                CreateComponentBatch<T1>(entities),
                CreateComponentBatch<T2>(entities),
                CreateComponentBatch<T3>(entities),
                CreateComponentBatch<T4>(entities)
            };
            
            EntityChangeRouter.PublishEntityAddedComponents(entities, componentTypeIds);
        }

        public void CreateComponents<T1, T2, T3, T4, T5>(Entity[] entities) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new()
        {
            var componentTypeIds = new[]
            {
                CreateComponentBatch<T1>(entities),
                CreateComponentBatch<T2>(entities),
                CreateComponentBatch<T3>(entities),
                CreateComponentBatch<T4>(entities),
                CreateComponentBatch<T5>(entities)
            };
            
            EntityChangeRouter.PublishEntityAddedComponents(entities, componentTypeIds);
        }

        public void CreateComponents<T1, T2, T3, T4, T5, T6>(Entity[] entities) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new() where T6 : IComponent, new()
        {
            var componentTypeIds = new[]
            {
                CreateComponentBatch<T1>(entities),
                CreateComponentBatch<T2>(entities),
                CreateComponentBatch<T3>(entities),
                CreateComponentBatch<T4>(entities),
                CreateComponentBatch<T5>(entities),
                CreateComponentBatch<T6>(entities),
            };
            
            EntityChangeRouter.PublishEntityAddedComponents(entities, componentTypeIds);
        }
        
        public void CreateComponents<T1, T2, T3, T4, T5, T6, T7>(Entity[] entities) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new() where T6 : IComponent, new() where T7 : IComponent, new()
        {
            var componentTypeIds = new[]
            {
                CreateComponentBatch<T1>(entities),
                CreateComponentBatch<T2>(entities),
                CreateComponentBatch<T3>(entities),
                CreateComponentBatch<T4>(entities),
                CreateComponentBatch<T5>(entities),
                CreateComponentBatch<T6>(entities),
                CreateComponentBatch<T7>(entities),
            };
            
            EntityChangeRouter.PublishEntityAddedComponents(entities, componentTypeIds);
        }
        
        public void RemoveComponents(Entity entity, IReadOnlyList<int> componentsTypeIds)
        {
            var lastIndex = 0;
            Span<int> temporaryComponentIds = stackalloc int[componentsTypeIds.Count];
            for (var i = 0; i < componentsTypeIds.Count; i++)
            {
                var componentTypeId = componentsTypeIds[i];
                if(EntityAllocationDatabase.HasComponent(componentTypeId, entity))
                { temporaryComponentIds[lastIndex++] = componentsTypeIds[i]; }
            }
            if(temporaryComponentIds.Length == 0) { return; }
            var sanitisedComponentsIds = temporaryComponentIds[..lastIndex].ToArray();
            
            EntityChangeRouter.PublishEntityRemovingComponents(entity, sanitisedComponentsIds);
 
            for (var i = 0; i < sanitisedComponentsIds.Length; i++)
            {
                var componentId = sanitisedComponentsIds[i];
                var allocationIndex = EntityAllocationDatabase.ReleaseComponent(componentId, entity);
                ComponentDatabase.Remove(componentId, allocationIndex);
            }
            
            EntityChangeRouter.PublishEntityRemovedComponents(entity, sanitisedComponentsIds);
        }

        public void RemoveComponents(Entity entity, IReadOnlyList<Type> componentTypes)
        {
            var componentTypeIds = ComponentTypeLookup.GetComponentTypeIds(componentTypes);
            RemoveComponents(entity, componentTypeIds);
        }

        public void RemoveAllComponents(Entity entity)
        {
            var allComponentIds = EntityAllocationDatabase.GetAllocatedComponentTypes(entity);
            RemoveComponents(entity, allComponentIds);
        }
        
        public IEnumerable<IComponent> GetComponents(Entity entity)
        {
            var componentAllocations = EntityAllocationDatabase.GetEntityAllocations(entity);
            for (var i = 0; i < componentAllocations.Length; i++)
            {
                if(componentAllocations[i] == IEntityAllocationDatabase.NoAllocation) { continue; }

                var allocationIndex = componentAllocations[i];
                yield return ComponentDatabase.Get(i, allocationIndex);
            }
        }

        public IComponent GetComponent(Entity entity, Type componentType)
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(componentType);
            var allocationId = EntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, entity);
            if(allocationId == IEntityAllocationDatabase.NoAllocation) 
            { throw new Exception($"Component [{componentTypeId}] not found for entity [{entity.Id}]"); }
            
            return ComponentDatabase.Get(componentTypeId, allocationId);
        }
        
        public T[] GetComponent<T>(Entity[] entities) where T : IComponent, new()
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(typeof(T));
            var allocationIds = EntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, entities);

            for (var i = 0; i < allocationIds.Length; i++)
            {
                if(allocationIds[i] == IEntityAllocationDatabase.NoAllocation) 
                { throw new Exception($"Component [{componentTypeId}] not found for entity [{entities[i]}]"); }
            }
            
            return ComponentDatabase.Get<T>(componentTypeId, allocationIds);
        }
        
        public ref T GetComponentRef<T>(Entity entity) where T : struct, IComponent
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(typeof(T));
            var allocationId = EntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, entity);
            if(allocationId == IEntityAllocationDatabase.NoAllocation) 
            { throw new Exception($"Component [{componentTypeId}] not found for entity [{entity}]"); }
            
            return ref ComponentDatabase.GetRef<T>(componentTypeId, allocationId);
        }
        
        public RefBuffer<T> GetComponentRef<T>(Entity[] entities) where T : struct, IComponent
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(typeof(T));
            var allocationIds = EntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, entities);
            return ComponentDatabase.GetRef<T>(componentTypeId, allocationIds);
        }

        public void UpdateComponent<T>(Entity entity, T newValue) where T : struct, IComponent
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(typeof(T));
            var allocationId = EntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, entity);;
            ComponentDatabase.Set(componentTypeId, allocationId, newValue);
        }

        public bool HasComponent(Entity entity, Type componentType)
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(componentType);
            return EntityAllocationDatabase.HasComponent(componentTypeId, entity);
        }
        
        public int[] GetAllocations(Entity entity)
        { return EntityAllocationDatabase.GetEntityAllocations(entity); }

        public bool HasAllComponents(Entity entity, IReadOnlyList<Type> componentTypes)
        {
            for(var i=0;i<componentTypes.Count;i++)
            {
                var componentType = componentTypes[i];
                if(!HasComponent(entity, componentType)) { return false; }
            }
            return true;
        }

        public bool HasAnyComponents(Entity entity, IReadOnlyList<Type> componentTypes)
        {
            for(var i=0;i<componentTypes.Count;i++)
            {
                var componentType = componentTypes[i];
                if(HasComponent(entity, componentType)) { return true; }
            }
            return false;
        }

        public bool IsEntityValid(Entity entity)
        {
            if(entity.Id < 0) { return false; }

            var possibleEntity = EntityAllocationDatabase.GetEntity(entity.Id);
            if(possibleEntity == null) { return false; }

            return entity.CreationHash == possibleEntity.Value.CreationHash;
        }
    }
}