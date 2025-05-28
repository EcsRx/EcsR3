using EcsR3.Components;
using EcsR3.Entities.Accessors;

namespace EcsR3.Extensions
{
    public static class IEntityComponentAccessorExtensions
    {
        /// <summary>
        /// Checks to see if the entity contains the given component based on its type id
        /// </summary>
        /// <param name="entityId">The entity Id to check on</param>
        /// <returns>true if the component can be found, false if it cant be</returns>
        public static bool HasComponent<T>(this IEntityComponentAccessor accessor, int entityId) where T : IComponent
        {
            return accessor.HasComponent(entityId, typeof(T));
        }

        public static T GetComponent<T>(this IEntityComponentAccessor accessor, int entityId) where T : IComponent
        {
            return (T)accessor.GetComponent(entityId, typeof(T));
        }
        
        public static void RemoveComponent(this IEntityComponentAccessor accessor, int entityId, IComponent component)
        { accessor.RemoveComponents(entityId, component.GetType()); }
        
        public static void RemoveComponent<T>(this IEntityComponentAccessor accessor, int entityId) where T : IComponent
        { accessor.RemoveComponents(entityId, typeof(T)); }
    }
}