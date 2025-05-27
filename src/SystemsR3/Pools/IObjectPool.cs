using System.Collections.Generic;

namespace SystemsR3.Pools
{
    public interface IObjectPool<T> : IPool<T>, IEnumerable<T>
        where T : class
    {
        void PreAllocate(int? allocationAmount = null);
        
        void Clear();
    }
}