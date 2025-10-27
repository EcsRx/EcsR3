using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsR3.Components.Lookups
{
    public class ComponentTypeLookup : IComponentTypeLookup
    {
        public IReadOnlyDictionary<Type, int> ComponentsByType { get; }
        public IReadOnlyDictionary<int, Type> ComponentsById { get; }
        public bool[] ComponentStructLookups { get; }
        public bool[] ComponentDisposableLookups { get; }
        public int[] AllComponentTypeIds { get; }

        public ComponentTypeLookup(IReadOnlyDictionary<Type, int> componentsByType)
        {
            ComponentsByType = componentsByType;
            ComponentsById = componentsByType.ToDictionary(x => x.Value, x => x.Key);
            AllComponentTypeIds = componentsByType.Values.ToArray();
            
            ComponentStructLookups = componentsByType.Keys
                .Select(x => x.IsValueType)
                .ToArray();

            ComponentDisposableLookups = componentsByType.Keys
                .Select(x => x.GetInterfaces().Any(y => y == typeof(IDisposable)))
                .ToArray();
        }

        public int GetComponentTypeId(Type type)
        {
            try
            {
                return ComponentsByType[type];
            }
            catch (KeyNotFoundException ex) when (!typeof(IComponent).IsAssignableFrom(type))
            {
                throw new ArgumentException($"The supplied {nameof(type)} doesn't implement {nameof(IComponent)}. Additionally, there was no componentId was assigned to it. type = {type}", nameof(type), ex);
            }
        }

        public int[] GetComponentTypeIds(Type[] type)
        {
            var ids = new int[type.Length];
            for (var i = 0; i < type.Length; i++)
            { ids[i] = ComponentsByType[type[i]]; }
            return ids;
        }
        
        public int[] GetComponentTypeIds(IComponent[] components)
        {
            var ids = new int[components.Length];
            for (var i = 0; i < components.Length; i++)
            { ids[i] = ComponentsByType[components[i].GetType()]; }
            return ids;
        }

        public Type GetComponentType(int typeId)
        { return ComponentsById[typeId]; }

        public Type[] GetComponentTypes(int[] typeIds)
        {
            var types = new Type[typeIds.Length];
            for (var i = 0; i < typeIds.Length; i++)
            { types[i] = ComponentsById[typeIds[i]]; }
            return types;
        }

        public bool IsComponentStruct(int componentTypeId)
        { return ComponentStructLookups[componentTypeId]; }

        public bool IsComponentDisposable(int componentTypeId)
        { return ComponentDisposableLookups[componentTypeId]; }

        public IReadOnlyDictionary<Type, int> GetComponentTypeMappings()
        { return ComponentsByType; }
    }
}