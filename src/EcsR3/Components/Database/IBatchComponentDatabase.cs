using System.Collections.Generic;

namespace EcsR3.Components.Database
{
    public interface IBatchComponentDatabase
    {
        int[] Allocate(int componentTypeId, int count);
        void Remove(int componentTypeId, int[] allocationIndexes);
        void Set<T>(int componentTypeId, int[] allocationIndexes, IReadOnlyList<T> components) where T : IComponent;
        void SetMany(int[] componentTypeIds, int[] allocationIndexes, IReadOnlyList<IComponent> components);
    }
}