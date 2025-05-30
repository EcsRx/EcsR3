using EcsR3.Blueprints;
using EcsR3.Collections.Entities;
using EcsR3.Entities.Accessors;
using SystemsR3.Pools.Config;

namespace EcsR3.Pools
{
    /// <summary>
    /// Allows you to pool entities which are pre-configured with a blueprint then override the
    /// OnAllocated and OnReleased methods to do the custom logic you need for when they are
    /// re-assigned for use or released from use.
    /// </summary>
    /// <typeparam name="T">A blueprint that has a default constructor</typeparam>
    public class BlueprintedEntityPool<T> : EntityPool where T : IBlueprint, new()
    {
        public T Blueprint { get; } = new T();

        public BlueprintedEntityPool(IEntityCollection entityCollection, IEntityComponentAccessor entityComponentAccessor, PoolConfig poolConfig = null) : base(entityCollection, entityComponentAccessor, poolConfig)
        {}
        
        public override void SetupEntity(int entityId)
        { Blueprint.Apply(EntityComponentAccessor, entityId); }
    }
}