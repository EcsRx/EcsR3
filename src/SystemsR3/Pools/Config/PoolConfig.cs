namespace SystemsR3.Pools.Config
{
    /// <summary>
    /// Default configuration for how a pool should size and expand
    /// </summary>
    /// <remarks>Not every pool type supports every config parameter, or will automatically initialize to the given size without explicit preallocation calls</remarks>
    public class PoolConfig
    {
        /// <summary>
        /// The amount to expand by if not provided an override
        /// </summary>
        public int ExpansionSize { get;  }
        
        /// <summary>
        /// The initial size to preallocate to if needed
        /// </summary>
        public int InitialSize { get;  }
        
        /// <summary>
        /// The maximum size to make the pool if needed
        /// </summary>
        public int MaxSize { get; }

        public PoolConfig(int initialSize = 100, int expansionSize = 500, int maxSize = int.MaxValue)
        {
            InitialSize = initialSize;
            ExpansionSize = expansionSize;
            MaxSize = maxSize;
        }
    }
}