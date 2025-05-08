namespace SystemsR3.Pools
{
    public interface IPool<T>
    {
        /// <summary>
        /// The pool tuning config which describes how something should expand, initially size etc
        /// </summary>
        PoolConfig PoolConfig { get; }
        
        /// <summary>
        /// Allocates an instance in the pool for use
        /// </summary>
        /// <returns>An instance to use</returns>
        T AllocateInstance();
        
        /// <summary>
        /// Frees up the pooled item for re-allocation
        /// </summary>
        /// <param name="obj">The instance to put back in the pool</param>
        void ReleaseInstance(T obj);
    }
}