using System;
using System.Collections.Generic;
using EcsR3.Components;

namespace EcsR3.Entities
{
    /// <summary>
    /// A container for components, its only job is to let you compose components
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The Id of the entity
        /// </summary>
        /// <remarks>
        /// It is recommended you do not pass entities around and instead pass their ids around
        /// and then use the collection/observable group methods to get the entity from its id
        /// </remarks>
        int Id { get; }
        
        /// <summary>
        /// All the components which have been applied to this entity
        /// </summary>
        IEnumerable<IComponent> Components { get; }

        /// <summary>
        /// All allocations of components in the component database
        /// </summary>
        IReadOnlyList<int> ComponentAllocations { get; }

        /// <summary>
        /// Adds all provided components to the entity
        /// </summary>
        /// <param name="components">The components to add</param>
        void AddComponents(IReadOnlyList<IComponent> components);
        
        /// <summary>
        /// Removes component types from the entity
        /// </summary>
        /// <param name="componentsTypes">The component types to remove</param>
        void RemoveComponents(params Type[] componentsTypes);
        
        /// <summary>
        /// Removes all the components from the entity
        /// </summary>
        void RemoveAllComponents();
               
        /// <summary>
        /// Gets a component from the entity based upon its type or null if one cannot be found
        /// </summary>
        /// <param name="componentType">The type of component to retrieve</param>
        /// <returns>The component instance if found, or null if not</returns>
        IComponent GetComponent(Type componentType);        
        
        /// <summary>
        /// Gets a component from its type id
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <returns>The ref of the component</returns>
        /// <remarks>This is meant for struct based components</remarks>
        ref T GetComponentRef<T>() where T : struct, IComponent;
        
        /// <summary>
        /// Creates and returns a struct type component
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <returns>The ref of the component</returns>
        /// <remarks>This is meant for struct based components and is used instead of AddComponent</remarks>
        ref T CreateComponent<T>() where T : struct, IComponent;
        
        /// <summary>
        /// Updates a component from its type id with the new values
        /// </summary>
        /// <param name="newValue">The struct containing new values</param>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <remarks>This is meant for struct based components</remarks>
        void UpdateComponent<T>(T newValue) where T : struct, IComponent;
        
        /// <summary>
        /// Removes all components with matching type ids
        /// </summary>
        /// <param name="componentsTypeIds">The component type ids</param>
        void RemoveComponents(IReadOnlyList<int> componentsTypeIds);
        
        /// <summary>
        /// Checks to see if the entity contains the given component type
        /// </summary>
        /// <param name="componentType">Type of component to look for</param>
        /// <returns>true if the component can be found, false if it cant be</returns>
        bool HasComponent(Type componentType);
    }
}
