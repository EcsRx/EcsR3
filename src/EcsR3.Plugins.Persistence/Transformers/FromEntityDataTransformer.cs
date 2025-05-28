using EcsR3.Plugins.Persistence.Data;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;

namespace EcsR3.Plugins.Persistence.Transformers
{
    public class FromEntityDataTransformer : IFromEntityDataTransformer
    {
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        public FromEntityDataTransformer(IEntityComponentAccessor entityComponentAccessor)
        {
            EntityComponentAccessor = entityComponentAccessor;
        }

        public object Transform(object converted)
        {
            var entityData = (EntityData) converted;
            EntityComponentAccessor.AddComponents(entityData.EntityId, entityData.Components.ToArray());
            return entityData.EntityId;
        }
    }
}