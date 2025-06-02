using EcsR3.Entities;
using EcsR3.Entities.Accessors;

namespace EcsR3.Blueprints
{
    public interface IBatchedBlueprint
    {
        /// <summary>
        /// Applies the given blueprint to the entity
        /// </summary>
        /// <param name="entity">The entity to be configured</param>
        void Apply(IEntityComponentAccessor entityComponentAccessor, Entity[] entities);
    }
}