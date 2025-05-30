using System.Linq;
using EcsR3.Plugins.Persistence.Data;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;

namespace EcsR3.Plugins.Persistence.Transformers
{
    public class ToEntityDataTransformer : IToEntityDataTransformer
    {
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        public ToEntityDataTransformer(IEntityComponentAccessor entityComponentAccessor)
        {
            EntityComponentAccessor = entityComponentAccessor;
        }

        public object Transform(object original)
        {
            var entityId = (int)original;
            return new EntityData
            {
                EntityId = entityId,
                Components = EntityComponentAccessor.GetComponents(entityId).ToList()
            };
        }
    }
}