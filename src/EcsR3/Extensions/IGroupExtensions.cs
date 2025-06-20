﻿using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Components;
using EcsR3.Groups;

namespace EcsR3.Extensions
{
    public static class IGroupExtensions
    {
        public static IGroup WithComponent<T>(this IGroup group) where T : class, IComponent
        {
            var requiredComponents = new List<Type>(group.RequiredComponents) {typeof(T)};
            return new Group(requiredComponents, group.ExcludedComponents);
        }
        
        public static IGroup WithComponents(this IGroup group, params Type[] requiredComponents)
        {
            var newComponents = new List<Type>(group.RequiredComponents);
            newComponents.AddRange(requiredComponents);
            return new Group(newComponents, group.ExcludedComponents);
        }
        
        public static IGroup WithoutComponent<T>(this IGroup group) where T : class, IComponent
        {
            var excludedComponents = new List<Type>(group.ExcludedComponents) {typeof(T)};
            return new Group(group.RequiredComponents, excludedComponents);
        }
        
        public static IGroup WithoutComponent(this IGroup group, params Type[] excludedComponents)
        {
            var newComponents = new List<Type>(group.ExcludedComponents);
            newComponents.AddRange(excludedComponents);
            return new Group(group.RequiredComponents, newComponents);
        }
        
        public static bool ContainsAllRequiredComponents(this IGroup group, IEnumerable<IComponent> components)
        {
            for (var i = group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                if(!components.Any(x => group.RequiredComponents[i].IsInstanceOfType(x)))
                { return false; }
            }
            return true;
        }
               
        public static bool ContainsAllRequiredComponents(this IGroup group, params Type[] componentTypes)
        {
            for (var i = group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                if(!componentTypes.Contains(group.RequiredComponents[i]))
                { return false; }
            }
            
            return true;
        }
        
        public static bool ContainsAnyRequiredComponents(this IGroup group, IEnumerable<IComponent> components)
        {
            for (var i = group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                if(components.Any(x => group.RequiredComponents[i] == x.GetType()))
                { return true; }
            }
            return false;
        }

        public static bool ContainsAnyRequiredComponents(this IGroup group, params Type[] componentTypes)
        {

            for (var i = @group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                for (var j = componentTypes.Length - 1; j >= 0; j--)
                {
                    if(group.RequiredComponents[i] == componentTypes[j])
                    { return true; }
                }
            }

            return false;
        }
        
        public static bool ContainsAnyExcludedComponents(this IGroup group, IEnumerable<IComponent> components)
        {
            var castComponents = components.Select(x => x.GetType()).ToArray();
            return ContainsAnyExcludedComponents(group, castComponents);
        }
        
        public static bool ContainsAnyExcludedComponents(this IGroup group, params Type[] componentTypes)
        { return group.ExcludedComponents.Any(componentTypes.Contains); }
        
        public static bool ContainsAny(this IGroup group, params IComponent[] components)
        {
            var requiredContains = group.ContainsAnyRequiredComponents(components);
            if(requiredContains) { return true; }

            return group.ContainsAnyExcludedComponents(components);
        }
        
        public static bool ContainsAny(this IGroup group, params Type[] componentTypes)
        {
            var requiredContains = group.RequiredComponents.Any(componentTypes.Contains);
            
            if(requiredContains) { return true; }

            return group.ExcludedComponents.Any(componentTypes.Contains);
        }
        
        /// <summary>
        /// Creates empty components based on the required components for this group
        /// </summary>
        /// <param name="group">The group to take required components from</param>
        /// <returns>The newly created components</returns>
        public static IReadOnlyList<IComponent> CreateRequiredComponents(this IGroup group)
        {
            return group.RequiredComponents
                .Select(Activator.CreateInstance)
                .Cast<IComponent>()
                .ToArray();
        }
    }
}