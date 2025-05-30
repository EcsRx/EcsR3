using System.Collections.Generic;
using SystemsR3.Utility;

namespace EcsR3.Components.Database
{
    public interface IBatchComponentDatabase
    {
        int[] Allocate(int componentTypeId, int count);
        T[] Get<T>(int componentTypeId, int[] allocationIndex) where T : IComponent;
        RefBuffer<T> GetRef<T>(int componentTypeId, int[] allocationIndexes) where T : struct, IComponent;
        void Remove(int componentTypeId, int[] allocationIndexes);
        void Set<T>(int componentTypeId, int[] allocationIndexes, IReadOnlyList<T> components) where T : IComponent;
        void SetMany(int[] componentTypeIds, int[] allocationIndexes, IReadOnlyList<IComponent> components);
    }
}