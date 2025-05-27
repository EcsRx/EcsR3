using System;
using System.Collections.Generic;
using EcsR3.Components;
using EcsR3.Entities.Accessors;

namespace EcsR3.Entities
{
    public class Entity : IEntity
    {
        public int Id { get; }
        
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        public IReadOnlyList<int> ComponentAllocations => EntityComponentAccessor.GetAllocations(Id);

        public IEnumerable<IComponent> Components => EntityComponentAccessor.GetComponents(Id);
        
        public Entity(int id, IEntityComponentAccessor entityComponentAccessor)
        {
            Id = id;
            EntityComponentAccessor = entityComponentAccessor;
        }
        
        public void AddComponents(IReadOnlyList<IComponent> components)
        { EntityComponentAccessor.AddComponents(Id, components); }

        public ref T CreateComponent<T>() where T : struct, IComponent
        { return ref EntityComponentAccessor.CreateComponent<T>(Id); }

        public void UpdateComponent<T>(T newValue) where T : struct, IComponent
        { EntityComponentAccessor.UpdateComponent(Id, newValue); }
        
        public void RemoveComponents(params Type[] componentTypes)
        { EntityComponentAccessor.RemoveComponents(Id, componentTypes); }
        
        public void RemoveComponents(IReadOnlyList<int> componentsTypeIds)
        { EntityComponentAccessor.RemoveComponents(Id, componentsTypeIds); }

        public void RemoveAllComponents()
        { EntityComponentAccessor.RemoveAllComponents(Id); }

        public bool HasComponent(Type componentType)
        { return EntityComponentAccessor.HasComponent(Id, componentType); }

        public IComponent GetComponent(Type componentType)
        { return EntityComponentAccessor.GetComponent(Id, componentType); }

        public ref T GetComponentRef<T>() where T : struct, IComponent
        { return ref EntityComponentAccessor.GetComponentRef<T>(Id); }

        public override int GetHashCode()
        { return Id; }
    }
}
