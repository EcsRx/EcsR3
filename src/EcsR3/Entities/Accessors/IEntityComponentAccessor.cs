using System;
using System.Collections.Generic;
using EcsR3.Components;

namespace EcsR3.Entities.Accessors
{
    public interface IEntityComponentAccessor : IEntityStructComponentAccessor, IBatchEntityComponentAccessor
    {
        /// <summary>
        /// Adds all provided components to the entity
        /// </summary>
        /// <param name="components">The components to add</param>
        void AddComponents(Entity entity, IComponent[] components);
 
        /// <summary>
        /// Removes component types from the entity
        /// </summary>
        /// <param name="componentsTypes">The component types to remove</param>
        void RemoveComponents(Entity entity, Type[] components);
        
        /// <summary>
        /// Removes all the components from the entity
        /// </summary>
        void RemoveAllComponents(Entity entity);
               
        /// <summary>
        /// Gets a component from the entity based upon its type or null if one cannot be found
        /// </summary>
        /// <param name="componentType">The type of component to retrieve</param>
        /// <returns>The component instance if found, or null if not</returns>
        IComponent GetComponent(Entity entity, Type componentType);
        
        /// <summary>
        /// Gets all components for an entity
        /// </summary>
        /// <param name="entityId">The entity Id</param>
        /// <returns>All components the entity owns</returns>
        IEnumerable<IComponent> GetComponents(Entity entity);
        
        /// <summary>
        /// Removes all components with matching type ids
        /// </summary>
        /// <param name="componentsTypeIds">The component type ids</param>
        void RemoveComponents(Entity entity, int[] componentTypeIds);
        
        /// <summary>
        /// Checks to see if the entity contains the given component type
        /// </summary>
        /// <param name="componentType">Type of component to look for</param>
        /// <returns>true if the component can be found, false if it cant be</returns>
        bool HasComponent(Entity entity, Type componentType);

        /// <summary>
        /// Get all allocations for an entity
        /// </summary>
        /// <param name="entityId">The entity id to query on</param>
        /// <returns></returns>
        int[] GetAllocations(Entity entity);
        
        /// <summary>
        /// Checks to see if the entity contains all the given component types
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="componentTypes"></param>
        /// <returns></returns>
        bool HasAllComponents(Entity entity, Type[] componentTypes);
        
        /// <summary>
        /// Checks to see if the entity contains any of the given component types
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="componentTypes"></param>
        /// <returns></returns>
        bool HasAnyComponents(Entity entity, Type[] componentTypes);
        
        /// <summary>
        /// Verifies the entity is valid, as you may have an entity which has been released elsewhere
        /// </summary>
        /// <param name="entity">The entity to check</param>
        /// <returns>true if the entity is currently active, false if its not</returns>
        bool IsEntityValid(Entity entity);
    }
}