using System;
using System.Collections.Generic;

namespace EcsR3.Components.Lookups
{
    /// <summary>
    /// The Component Type Lookup interface is responsible for looking up
    /// component ids for the types as well as vice versa. 
    /// </summary>
    public interface IComponentTypeLookup
    {
        int[] AllComponentTypeIds { get; }
        IReadOnlyDictionary<Type, int> GetComponentTypeMappings();
        int GetComponentTypeId(Type type);
        int[] GetComponentTypeIds(IReadOnlyList<Type> type);
        int[] GetComponentTypeIds(IReadOnlyList<IComponent> components);
        Type GetComponentType(int typeId);
        Type[] GetComponentTypes(IReadOnlyList<int> typeIds);
        bool IsComponentStruct(int componentTypeId);
        bool IsComponentDisposable(int componentTypeId);
    }
}