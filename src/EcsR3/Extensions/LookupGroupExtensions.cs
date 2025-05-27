using System.Collections.Generic;
using System.Linq;
using EcsR3.Entities;
using EcsR3.Groups;

namespace EcsR3.Extensions
{
    public static class LookupGroupExtensions
    {                
        public static bool ContainsAllRequiredComponents(this LookupGroup group, IReadOnlyList<int> componentTypeIds)
        {
            for (var i = group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                if(!componentTypeIds.Contains(group.RequiredComponents[i]))
                { return false; }
            }
            
            return true;
        }
        
        public static bool ContainsAnyRequiredComponents(this LookupGroup group, IReadOnlyList<int> componentTypes)
        {
            for (var i = group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                for (var j = componentTypes.Count - 1; j >= 0; j--)
                {
                    if(group.RequiredComponents[i] == componentTypes[j])
                    { return true; }
                }
            }

            return false;
        }
        
        public static bool ContainsAnyExcludedComponents(this LookupGroup group, IReadOnlyList<int> componentTypes)
        { return group.ExcludedComponents.Any(componentTypes.Contains); }
        
        public static bool Matches(this LookupGroup lookupGroup, params int[] componentTypes)
        { return Matches(lookupGroup, (IReadOnlyList<int>) componentTypes); }
        
        public static bool Matches(this LookupGroup lookupGroup, IReadOnlyList<int> componentTypes)
        {
            if(lookupGroup.ExcludedComponents.Length == 0)
            { return ContainsAllRequiredComponents(lookupGroup, componentTypes); }
            
            return ContainsAllRequiredComponents(lookupGroup, componentTypes) && !ContainsAnyExcludedComponents(lookupGroup, componentTypes);
        }
    }
}