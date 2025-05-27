using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Components;
using EcsR3.Components.Lookups;
using EcsR3.Groups;

namespace EcsR3.Extensions
{
    public static class IComponentTypeLookupExtensions
    {
        public static int GetComponentType<T>(this IComponentTypeLookup typeLookup) where T : IComponent
        { return typeLookup.GetComponentTypeId(typeof(T)); }
        
        public static T CreateDefault<T>(this IComponentTypeLookup typeLookup) where T : IComponent, new()
        { return new T(); }

        public static IComponent CreateDefault(this IComponentTypeLookup typeLookup, int typeId)
        { return Activator.CreateInstance(typeLookup.GetComponentType(typeId)) as IComponent; }
        
        public static int[] GetComponentTypeIds(this IComponentTypeLookup typeLookup, params Type[] types)
        { return types.Select(typeLookup.GetComponentTypeId).ToArray(); }

        public static Type[] GetComponentTypes(this IComponentTypeLookup typeLookup, params int[] typeIds)
        { return typeIds.Select(typeLookup.GetComponentType).ToArray(); }
        
        public static int[] GetComponentTypes(this IComponentTypeLookup typeLookup, IEnumerable<Type> types)
        { return types.Select(typeLookup.GetComponentTypeId).ToArray(); }

        public static Type[] GetComponentTypes(this IComponentTypeLookup typeLookup, IEnumerable<int> typeIds)
        { return typeIds.Select(typeLookup.GetComponentType).ToArray(); }

        public static LookupGroup GetLookupGroupFor(this IComponentTypeLookup typeLookup, IGroup group)
        {
            var requiredComponents = typeLookup.GetComponentTypeIds(group.RequiredComponents);
            var excludedComponents = typeLookup.GetComponentTypeIds(group.ExcludedComponents);
            return new LookupGroup(requiredComponents, excludedComponents);
        }
        
        public static IGroup GetGroupFor(this IComponentTypeLookup typeLookup, LookupGroup group)
        {
            var requiredComponents = typeLookup.GetComponentTypes(group.RequiredComponents);
            var excludedComponents = typeLookup.GetComponentTypes(group.ExcludedComponents);
            return new Group(requiredComponents, excludedComponents);
        }
        
        public static LookupGroup GetLookupGroupFor(this IComponentTypeLookup typeLookup, params Type[] componentTypes)
        { return new LookupGroup(GetComponentTypeIds(typeLookup, componentTypes), Array.Empty<int>()); }
    }
}