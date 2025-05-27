using System.Collections.Generic;

namespace SystemsR3.Pools
{
    public interface IBatchPool<T>
    {
        T[] AllocateMany(int count);
        void ReleaseMany(IReadOnlyList<T> instances);
    }
}