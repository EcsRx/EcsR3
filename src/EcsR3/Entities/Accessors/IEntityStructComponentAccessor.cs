using EcsR3.Components;

namespace EcsR3.Entities.Accessors
{
    public interface IEntityStructComponentAccessor
    {
        /// <summary>
        /// Creates and returns a struct type component
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <returns>The ref of the component</returns>
        /// <remarks>This is meant for struct based components and is used instead of AddComponent</remarks>
        ref T CreateComponent<T>(Entity entity) where T : struct, IComponent;
        
        /// <summary>
        /// Gets a component from its type id
        /// </summary>
        /// <param name="componentTypeId">The component type id</param>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <returns>The ref of the component</returns>
        /// <remarks>This is meant for struct based components</remarks>
        ref T GetComponentRef<T>(Entity entity) where T : struct, IComponent;
        
        /// <summary>
        /// Updates a component from its type id with the new values
        /// </summary>
        /// <param name="componentTypeId">The component type id</param>
        /// <param name="newValue">The struct containing new values</param>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <remarks>This is meant for struct based components</remarks>
        void UpdateComponent<T>(Entity entity, T newValue) where T : struct, IComponent;
    }
}