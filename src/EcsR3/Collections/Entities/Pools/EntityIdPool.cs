using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace EcsR3.Collections.Entities.Pools
{
    public class EntityIdPool : IdPool, IEntityIdPool
    {
        public EntityIdPool(PoolConfig poolConfig = null) : base(poolConfig)
        {
        }
    }
}