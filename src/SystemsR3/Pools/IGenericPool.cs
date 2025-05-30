using System.Collections.Generic;

namespace SystemsR3.Pools
{
    public interface IGenericPool<T> : IPool<T>, IEnumerable<T>
    {
        void PreAllocate(int? allocationAmount = null);
        
        void Clear();
    }
}