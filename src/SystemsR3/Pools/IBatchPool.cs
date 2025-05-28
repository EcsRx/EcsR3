using System.Collections.Generic;

namespace SystemsR3.Pools
{
    public interface IBatchPool<T>
    {
        T[] Allocate(int count);
        void Release(IReadOnlyList<T> instances);
    }
}