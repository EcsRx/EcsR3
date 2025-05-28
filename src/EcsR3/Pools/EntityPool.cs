using EcsR3.Collections.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace EcsR3.Pools
{
    /*
    /// <summary>
    /// Allows you to create a pool of entities which can be reused without having to constantly re-create them
    /// </summary>
    public abstract class EntityPool : ObjectPool<IEntity>
    {
        public IEntityCollection EntityCollection { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        protected EntityPool(IEntityCollection entityCollection, IEntityComponentAccessor entityComponentAccessor,
            PoolConfig poolConfig = null) : base(poolConfig)
        {
            EntityCollection = entityCollection;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public abstract void SetupEntity(int entityId);

        public override int Create()
        {
            var entity = EntityCollection.Create();
            SetupEntity(entity);
            return entity;
        }

        public override void Destroy(int entityId)
        { EntityComponentAccessor.RemoveAllComponents(entityId); }
    }*/
}