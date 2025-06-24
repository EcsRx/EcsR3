using System;
using System.Linq;
using EcsR3.Components;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;

namespace EcsR3.Extensions
{
    public static class IEntityComponentAccessorExtensions
    {
        /// <summary>
        /// Checks to see if the entity contains the given component based on its type id
        /// </summary>
        /// <param name="accessor">The entity accessor</param>
        /// <param name="entity">The entity to check on</param>
        /// <returns>true if the component can be found, false if it cant be</returns>
        public static bool HasComponent<T>(this IEntityComponentAccessor accessor, Entity entity) where T : IComponent
        { return accessor.HasComponent(entity, typeof(T)); }

        public static T GetComponent<T>(this IEntityComponentAccessor accessor, Entity entity) where T : IComponent
        { return (T)accessor.GetComponent(entity, typeof(T)); }
        
        public static void RemoveComponent(this IEntityComponentAccessor accessor, Entity entity, IComponent component)
        { accessor.RemoveComponents(entity, component.GetType()); }
        
        public static void RemoveComponent<T>(this IEntityComponentAccessor accessor, Entity entity) where T : IComponent
        { accessor.RemoveComponents(entity, typeof(T)); }
        
        public static void RemoveComponents(this IEntityComponentAccessor accessor, Entity entity, params Type[] componentTypes)
        { accessor.RemoveComponents(entity, componentTypes); }
        
        public static void RemoveComponents(this IEntityComponentAccessor accessor, Entity entity, params IComponent[] components)
        { accessor.RemoveComponents(entity, components.Select(x => x.GetType()).ToArray()); }
        
        public static void AddComponent(this IEntityComponentAccessor accessor, Entity entity, IComponent component)
        { accessor.AddComponents(entity, component); }
        
        public static void AddComponents(this IEntityComponentAccessor accessor, Entity entity, params IComponent[] components)
        { accessor.AddComponents(entity, components); }

        public static void CreateComponent<T>(this IEntityComponentAccessor accessor, Entity entity)
            where T : IComponent, new()
        { accessor.CreateComponent<T>(new[] { entity }); }
        
        public static void CreateComponents<T1, T2>(this IEntityComponentAccessor accessor, Entity entity) where T1 : IComponent, new() where T2 : IComponent, new()
        { accessor.CreateComponents<T1, T2>(new[] { entity }); }
        
        public static void CreateComponents<T1, T2, T3>(this IEntityComponentAccessor accessor, Entity entity) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new()
        { accessor.CreateComponents<T1, T2, T3>(new[] { entity }); }
        
        public static void CreateComponents<T1, T2, T3, T4>(this IEntityComponentAccessor accessor, Entity entity) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new()
        { accessor.CreateComponents<T1, T2, T3, T4>(new[] { entity }); }
        
        public static void CreateComponents<T1, T2, T3, T4, T5>(this IEntityComponentAccessor accessor, Entity entity) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new()
        { accessor.CreateComponents<T1, T2, T3, T4, T5>(new[] { entity }); }
        
        public static void CreateComponents<T1, T2, T3, T4, T5, T6>(this IEntityComponentAccessor accessor, Entity entity) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new() where T6 : IComponent, new()
        { accessor.CreateComponents<T1, T2, T3, T4, T5, T6>(new[] { entity }); }
        
        public static void CreateComponents<T1, T2, T3, T4, T5, T6, T7>(this IEntityComponentAccessor accessor, Entity entity) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new() where T6 : IComponent, new() where T7 : IComponent, new()
        { accessor.CreateComponents<T1, T2, T3, T4, T5, T6, T7>(new[] { entity }); }

        public static bool MatchesGroup(this IEntityComponentAccessor accessor, Entity entity, IGroup group)
        {
            if(!accessor.HasAllComponents(entity, group.RequiredComponents))
            { return false; }

            return group.ExcludedComponents.Length == 0 || !accessor.HasAnyComponents(entity, group.ExcludedComponents);
        }
    }
}