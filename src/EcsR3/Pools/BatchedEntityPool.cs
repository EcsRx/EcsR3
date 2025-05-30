using EcsR3.Collections.Entities;
using EcsR3.Entities.Accessors;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace EcsR3.Pools
{
    /// <summary>
    /// Allows you to create a pool of entities which can be reused without having to constantly re-create them
    /// </summary>
    public abstract class BatchedEntityPool : BatchedGenericPool<int>
    {
        public IEntityCollection EntityCollection { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        protected BatchedEntityPool(IEntityCollection entityCollection, IEntityComponentAccessor entityComponentAccessor,
            PoolConfig poolConfig = null) : base(poolConfig)
        {
            EntityCollection = entityCollection;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public abstract void SetupEntity(int[] entityIds);

        public override int[] Create(int count)
        {
            var entityIds = EntityCollection.CreateMany(count);
            SetupEntity(entityIds);
            return entityIds;
        }

        public override void Destroy(int[] entityIds)
        {
            for (var i = 0; i < entityIds.Length; i++)
            { EntityComponentAccessor.RemoveAllComponents(entityIds[i]); }
        }
    }
}