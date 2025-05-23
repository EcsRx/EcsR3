using EcsR3.Collections.Entity;
using EcsR3.Entities;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace EcsR3.Pools
{
    /// <summary>
    /// Allows you to create a pool of entities which can be reused without having to constantly re-create them
    /// </summary>
    public abstract class EntityPool : ObjectPool<IEntity>
    {
        public IEntityCollection EntityCollection { get; }

        protected EntityPool(IEntityCollection entityCollection, PoolConfig poolConfig = null) : base(poolConfig)
        { EntityCollection = entityCollection; }

        public abstract void SetupEntity(IEntity entity);

        public override IEntity Create()
        {
            var entity = EntityCollection.Create();
            SetupEntity(entity);
            return entity;
        }

        public override void Destroy(IEntity instance)
        {
            instance.RemoveAllComponents();
        }
    }
}