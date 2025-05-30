using System;
using System.Linq;
using EcsR3.Components;
using EcsR3.Entities.Accessors;

namespace EcsR3.Extensions
{
    public static class IEntityComponentAccessorExtensions
    {
        /// <summary>
        /// Checks to see if the entity contains the given component based on its type id
        /// </summary>
        /// <param name="accessor">The entity accessor</param>
        /// <param name="entityId">The entity Id to check on</param>
        /// <returns>true if the component can be found, false if it cant be</returns>
        public static bool HasComponent<T>(this IEntityComponentAccessor accessor, int entityId) where T : IComponent
        { return accessor.HasComponent(entityId, typeof(T)); }

        public static T GetComponent<T>(this IEntityComponentAccessor accessor, int entityId) where T : IComponent
        { return (T)accessor.GetComponent(entityId, typeof(T)); }
        
        public static void RemoveComponent(this IEntityComponentAccessor accessor, int entityId, IComponent component)
        { accessor.RemoveComponents(entityId, component.GetType()); }
        
        public static void RemoveComponent<T>(this IEntityComponentAccessor accessor, int entityId) where T : IComponent
        { accessor.RemoveComponents(entityId, typeof(T)); }
        
        public static void RemoveComponents(this IEntityComponentAccessor accessor, int entityId, params Type[] componentTypes)
        { accessor.RemoveComponents(entityId, componentTypes); }
        
        public static void RemoveComponents(this IEntityComponentAccessor accessor, int entityId, params IComponent[] components)
        { accessor.RemoveComponents(entityId, components.Select(x => x.GetType()).ToArray()); }
        
        public static void AddComponent(this IEntityComponentAccessor accessor, int entityId, IComponent component)
        { accessor.AddComponents(entityId, component); }
        
        public static void AddComponents(this IEntityComponentAccessor accessor, int entityId, params IComponent[] components)
        { accessor.AddComponents(entityId, components); }

        public static void CreateComponent<T>(this IEntityComponentAccessor accessor, int entityId)
            where T : IComponent, new()
        { accessor.CreateComponent<T>(new[] { entityId }); }
        
        public static void CreateComponents<T1, T2>(this IEntityComponentAccessor accessor, int entityId) where T1 : IComponent, new() where T2 : IComponent, new()
        { accessor.CreateComponents<T1, T2>(new[] { entityId }); }
        
        public static void CreateComponents<T1, T2, T3>(this IEntityComponentAccessor accessor, int entityId) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new()
        { accessor.CreateComponents<T1, T2, T3>(new[] { entityId }); }
        
        public static void CreateComponents<T1, T2, T3, T4>(this IEntityComponentAccessor accessor, int entityId) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new()
        { accessor.CreateComponents<T1, T2, T3, T4>(new[] { entityId }); }
        
        public static void CreateComponents<T1, T2, T3, T4, T5>(this IEntityComponentAccessor accessor, int entityId) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new()
        { accessor.CreateComponents<T1, T2, T3, T4, T5>(new[] { entityId }); }
        
        public static void CreateComponents<T1, T2, T3, T4, T5, T6>(this IEntityComponentAccessor accessor, int entityId) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new() where T6 : IComponent, new()
        { accessor.CreateComponents<T1, T2, T3, T4, T5, T6>(new[] { entityId }); }
        
        public static void CreateComponents<T1, T2, T3, T4, T5, T6, T7>(this IEntityComponentAccessor accessor, int entityId) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new() where T6 : IComponent, new() where T7 : IComponent, new()
        { accessor.CreateComponents<T1, T2, T3, T4, T5, T6, T7>(new[] { entityId }); }
    }
}