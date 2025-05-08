using EcsR3.Collections.Entity;
using EcsR3.Entities;
using SystemsR3.Pools;

namespace EcsR3.Pools
{
    /// <summary>
    /// Allows you to create a pool of entities which can be reused without having to constantly re-create them
    /// </summary>
    public abstract class EntityPool : ObjectPool<IEntity>
    {
        public IEntityCollection EntityCollection { get; }

        protected EntityPool(int expansionSize, IEntityCollection entityCollection) : base(expansionSize)
        {
            EntityCollection = entityCollection;
        }

        protected EntityPool(int expansionSize, int initialSize, IEntityCollection entityCollection) : base(expansionSize, initialSize)
        {
            EntityCollection = entityCollection;
        }

        public abstract void SetupEntity(IEntity entity);

        public override IEntity Create()
        {
            var entity = EntityCollection.CreateEntity();
            SetupEntity(entity);
            return entity;
        }

        public override void Destroy(IEntity instance)
        {
            instance.RemoveAllComponents();
        }
    }
}