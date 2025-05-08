using System;
using System.Collections.Generic;
using SystemsR3.Pools;

namespace EcsR3.Components.Database
{
    public class ComponentDatabaseConfig : PoolConfig
    {
        public bool OnlyPreAllocatePoolsWithConfig { get; set; } = false;
        public Dictionary<Type, PoolConfig> PoolSpecificConfig { get; } = new Dictionary<Type, PoolConfig>();

        public ComponentDatabaseConfig(int defaultInitialSize = 100, int defaultExpansionSize = 300, int defaultMaxSize = Int32.MaxValue) : base(defaultInitialSize, defaultExpansionSize, defaultMaxSize)
        {
        }
    }
}