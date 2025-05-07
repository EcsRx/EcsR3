using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities.Routing;
using EcsR3.Extensions;

namespace EcsR3.Entities
{
    public class Entity : IEntity
    {
        public static readonly int NotAllocated = -1;
        
        private readonly object _lock = new object();
        
        public int Id { get; }
        
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IEntityChangeRouter EntityChangeRouter { get; }
        
        public int[] InternalComponentAllocations { get; }
        public IReadOnlyList<int> ComponentAllocations => InternalComponentAllocations;

        public IEnumerable<IComponent> Components
        {
            get
            {
                lock (_lock)
                {
                    for (var componentTypeId = 0; componentTypeId < InternalComponentAllocations.Length; componentTypeId++)
                    {
                        if(InternalComponentAllocations[componentTypeId] != NotAllocated)
                        { yield return GetComponent(componentTypeId);}
                    }
                }
            }
        }
        
        public Entity(int id, IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IEntityChangeRouter entityChangeRouter)
        {
            Id = id;
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
            EntityChangeRouter = entityChangeRouter;

            var totalComponentCount = componentTypeLookup.AllComponentTypeIds.Length;
            InternalComponentAllocations = new int[totalComponentCount];
            
            EmptyAllAllocations();
        }

        public void EmptyAllAllocations()
        {
            lock (_lock)
            {
                for (var i = 0; i < InternalComponentAllocations.Length; i++)
                { InternalComponentAllocations[i] = NotAllocated; }
            }
        }
        
        public void AddComponents(IReadOnlyList<IComponent> components)
        {
            int[] componentTypeIds;
            lock (_lock)
            {
                componentTypeIds = new int[components.Count];
                for (var i = 0; i < components.Count; i++)
                {
                    var componentTypeId = ComponentTypeLookup.GetComponentTypeId(components[i].GetType());
                    var allocationId = ComponentDatabase.Allocate(componentTypeId);
                    InternalComponentAllocations[componentTypeId] = allocationId;
                    ComponentDatabase.Set(componentTypeId, allocationId, components[i]);
                    componentTypeIds[i] = componentTypeId;
                }
            }
            
            EntityChangeRouter.PublishEntityAddedComponents(Id, componentTypeIds);
        }

        public ref T AddComponent<T>(int componentTypeId) where T : IComponent, new()
        {
            var defaultComponent = ComponentTypeLookup.CreateDefault<T>();
            var allocationId = ComponentDatabase.Allocate(componentTypeId);

            lock (_lock)
            {
                InternalComponentAllocations[componentTypeId] = allocationId;
                ComponentDatabase.Set(componentTypeId, allocationId, defaultComponent);
            }
            
            EntityChangeRouter.PublishEntityAddedComponents(Id, new []{componentTypeId});
            return ref ComponentDatabase.GetRef<T>(componentTypeId, allocationId);
        }
        
        public void UpdateComponent<T>(int componentTypeId, T newValue) where T : struct, IComponent
        {
            lock (_lock)
            {
                var allocationId = InternalComponentAllocations[componentTypeId];
                ComponentDatabase.Set(componentTypeId, allocationId, newValue);
            }
        }
        
        public void RemoveComponents(params Type[] componentTypes)
        {
            var componentTypeIds = ComponentTypeLookup.GetComponentTypeIds(componentTypes);
            RemoveComponents(componentTypeIds);
        }
        
        public void RemoveComponents(IReadOnlyList<int> componentsTypeIds)
        {
            int[] sanitisedComponentsIds;

            lock (_lock)
            {
                var lastIndex = 0;
                Span<int> temporaryComponentIds = stackalloc int[componentsTypeIds.Count];
                for (var i = 0; i < componentsTypeIds.Count; i++)
                {
                    var componentTypeId = componentsTypeIds[i];
                    if(HasComponent(componentTypeId))
                    { temporaryComponentIds[lastIndex++] = componentsTypeIds[i]; }
                }
                if(temporaryComponentIds.Length == 0) { return; }
                sanitisedComponentsIds = temporaryComponentIds.Slice(0, lastIndex).ToArray();
            }
            
            EntityChangeRouter.PublishEntityRemovingComponents(Id, sanitisedComponentsIds);
            
            lock (_lock)
            {
                for (var i = 0; i < sanitisedComponentsIds.Length; i++)
                {
                    var componentId = sanitisedComponentsIds[i];
                    var allocationIndex = InternalComponentAllocations[componentId];
                    ComponentDatabase.Remove(componentId, allocationIndex);
                    InternalComponentAllocations[componentId] = NotAllocated;
                }
            }
            
            EntityChangeRouter.PublishEntityRemovedComponents(Id, sanitisedComponentsIds);
        }

        public void RemoveAllComponents()
        { RemoveComponents(ComponentTypeLookup.AllComponentTypeIds); }

        public bool HasComponent(Type componentType)
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(componentType);
            return HasComponent(componentTypeId);
        }

        public bool HasComponent(int componentTypeId)
        {
            lock (_lock)
            { return InternalComponentAllocations[componentTypeId] != NotAllocated; }
        }

        public IComponent GetComponent(Type componentType)
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(componentType);
            return GetComponent(componentTypeId);
        }

        public IComponent GetComponent(int componentTypeId)
        {
            lock (_lock)
            {
                var allocationIndex = InternalComponentAllocations[componentTypeId];
                return ComponentDatabase.Get(allocationIndex, componentTypeId);
            }
        }

        public ref T GetComponent<T>(int componentTypeId) where T : IComponent
        {
            lock (_lock)
            {
                var allocationIndex = InternalComponentAllocations[componentTypeId];
                return ref ComponentDatabase.GetRef<T>(componentTypeId, allocationIndex);
            }
        }

        public override int GetHashCode()
        { return Id; }
    }
}
