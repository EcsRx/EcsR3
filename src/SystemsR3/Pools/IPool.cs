using System;
using R3;
using SystemsR3.Pools.Config;

namespace SystemsR3.Pools
{
    public interface IPool<T> : IDisposable
    {
        /// <summary>
        /// The pool tuning config which describes how something should expand, initially size etc
        /// </summary>
        PoolConfig PoolConfig { get; }
        
        /// <summary>
        /// The current size of the pool
        /// </summary>
        int Size { get; }
        
        /// <summary>
        /// Observable to fire when the size of the pool has changed
        /// </summary>
        Observable<int> OnSizeChanged { get; }
        
        /// <summary>
        /// Allocates an instance in the pool for use
        /// </summary>
        /// <returns>An instance to use</returns>
        T Allocate();
        
        /// <summary>
        /// Frees up the pooled item for re-allocation
        /// </summary>
        /// <param name="instance">The instance to put back in the pool</param>
        void Release(T instance);
    }
}