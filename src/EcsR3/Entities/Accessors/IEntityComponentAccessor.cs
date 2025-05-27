using System;
using System.Collections.Generic;
using EcsR3.Components;

namespace EcsR3.Entities.Accessors
{
    public interface IEntityComponentAccessor
    {
        /// <summary>
        /// Adds all provided components to the entity
        /// </summary>
        /// <param name="components">The components to add</param>
        void AddComponents(int entityId, IReadOnlyList<IComponent> components);
        
        /// <summary>
        /// Creates and returns a struct type component
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <returns>The ref of the component</returns>
        /// <remarks>This is meant for struct based components and is used instead of AddComponent</remarks>
        ref T CreateComponent<T>(int entityId) where T : struct, IComponent;
        
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
        /// Gets a component from its type id
        /// </summary>
        /// <param name="componentTypeId">The component type id</param>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <returns>The ref of the component</returns>
        /// <remarks>This is meant for struct based components</remarks>
        T GetComponent<T>(int entityId) where T : IComponent;
        
        /// <summary>
        /// Gets a component from its type id
        /// </summary>
        /// <param name="componentTypeId">The component type id</param>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <returns>The ref of the component</returns>
        /// <remarks>This is meant for struct based components</remarks>
        ref T GetComponentRef<T>(int entityId) where T : struct, IComponent;
        
        /// <summary>
        /// Updates a component from its type id with the new values
        /// </summary>
        /// <param name="componentTypeId">The component type id</param>
        /// <param name="newValue">The struct containing new values</param>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <remarks>This is meant for struct based components</remarks>
        void UpdateComponent<T>(int entityId, T newValue) where T : struct, IComponent;
        
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
        
        /// <summary>
        /// Checks to see if the entity contains the given component based on its type id
        /// </summary>
        /// <param name="componentTypeId">Type id of component to look for</param>
        /// <returns>true if the component can be found, false if it cant be</returns>
        bool HasComponent<T>(int entityId);
    }
}