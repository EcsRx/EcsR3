using EcsR3.Collections.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace EcsR3.Pools
{
    /// <summary>
    /// Allows you to create a pool of entities which can be reused without having to constantly re-create them
    /// </summary>
    public abstract class BatchedEntityPool : BatchedGenericPool<Entity>
    {
        public IEntityCollection EntityCollection { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        protected BatchedEntityPool(IEntityCollection entityCollection, IEntityComponentAccessor entityComponentAccessor,
            PoolConfig poolConfig = null) : base(poolConfig)
        {
            EntityCollection = entityCollection;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public abstract void SetupEntity(Entity[] entities);

        public override Entity[] Create(int count)
        {
            var entities = EntityCollection.CreateMany(count);
            SetupEntity(entities);
            return entities;
        }

        public override void Destroy(Entity[] entities)
        {
            for (var i = 0; i < entities.Length; i++)
            { EntityComponentAccessor.RemoveAllComponents(entities[i]); }
        }
    }
}