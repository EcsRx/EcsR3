using System;
using System.Collections.Generic;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace EcsR3.Components.Database
{
    /// <summary>
    /// Configures how pools within the component database should expand, allowing you to customize how each component type should be created/expanded
    /// </summary>
    /// <remarks>
    /// The defaults should work fine for smaller projects, its highly recommended you tune each pool in production scenarios,
    /// i.e if you know you are definitely going to have 100000 entities of a given type in your game, pre allocate all 100000 up front
    /// for that pool, or if you have frequent churn work out the maximum churn size and put that as your expansion.
    /// Ultimately the more tuning you do, the less frequent minor allocations you will get.
    /// </remarks>
    public class ComponentDatabaseConfig : PoolConfig
    {
        /// <summary>
        /// This will not pre allocate on any pools other than the ones with specific configuration
        /// </summary>
        public bool OnlyPreAllocatePoolsWithConfig { get; set; } = false;
        
        /// <summary>
        /// This lets you provide configuration for each pool for a given type, it is optional, defaults will be used if not provided
        /// </summary>
        public Dictionary<Type, PoolConfig> PoolSpecificConfig { get; } = new Dictionary<Type, PoolConfig>();

        public ComponentDatabaseConfig(int defaultInitialSize = 100, int defaultExpansionSize = 500, int defaultMaxSize = Int32.MaxValue) : base(defaultInitialSize, defaultExpansionSize, defaultMaxSize)
        {
        }
    }
}