using EcsR3.Blueprints;
using EcsR3.Collections.Entity;
using EcsR3.Entities;

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

        public BlueprintedEntityPool(int expansionSize, IEntityCollection entityCollection) : base(expansionSize, entityCollection)
        {}

        public BlueprintedEntityPool(int expansionSize, int initialSize, IEntityCollection entityCollection) : base(expansionSize, initialSize, entityCollection)
        {}
        
        public override void SetupEntity(IEntity entity)
        { Blueprint.Apply(entity); }
    }
}