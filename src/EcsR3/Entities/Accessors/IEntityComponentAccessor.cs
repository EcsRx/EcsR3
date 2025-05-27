using System;
using System.Collections.Generic;
using EcsR3.Components;

namespace EcsR3.Entities.Accessors
{
    public interface IEntityComponentAccessor : IEntityStructComponentAccessor
    {
        /// <summary>
        /// Adds all provided components to the entity
        /// </summary>
        /// <param name="components">The components to add</param>
        void AddComponents(int entityId, IReadOnlyList<IComponent> components);
 
        /// <summary>
        /// Removes component types from the entity
        /// </summary>
        /// <param name="componentsTypes">The component types to remove</param>
        void RemoveComponents(int entityId, params Type[] componentTypes);
        
        /// <summary>
        /// Removes all the components from the entity
        /// </summary>
        void RemoveAllComponents(int entityId);
               
        /// <summary>
        /// Gets a component from the entity based upon its type or null if one cannot be found
        /// </summary>
        /// <param name="componentType">The type of component to retrieve</param>
        /// <returns>The component instance if found, or null if not</returns>
        IComponent GetComponent(int entityId, Type componentType);
        
        /// <summary>
        /// Gets all components for an entity
        /// </summary>
        /// <param name="entityId">The entity Id</param>
        /// <returns>All components the entity owns</returns>
        IEnumerable<IComponent> GetComponents(int entityId);
        
        /// <summary>
        /// Removes all components with matching type ids
        /// </summary>
        /// <param name="componentsTypeIds">The component type ids</param>
        void RemoveComponents(int entityId, IReadOnlyList<int> componentTypeIds);
        
        /// <summary>
        /// Checks to see if the entity contains the given component type
        /// </summary>
        /// <param name="componentType">Type of component to look for</param>
        /// <returns>true if the component can be found, false if it cant be</returns>
        bool HasComponent(int entityId, Type componentType);

        int[] GetAllocations(int entityId);
    }
}