using System.Collections.Generic;

namespace EcsR3.Components.Database
{
    public interface IBatchComponentDatabase
    {
        void SetMany(int[] componentTypeIds, int[] allocationIndexs, IReadOnlyList<IComponent> components);
    }
}