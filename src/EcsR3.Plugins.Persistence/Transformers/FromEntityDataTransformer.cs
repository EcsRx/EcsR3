using EcsR3.Collections.Entities;
using EcsR3.Plugins.Persistence.Data;
using EcsR3.Entities.Accessors;

namespace EcsR3.Plugins.Persistence.Transformers
{
    public class FromEntityDataTransformer : IFromEntityDataTransformer
    {
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        public IEntityAllocationDatabase EntityAllocationDatabase { get; }

        public FromEntityDataTransformer(IEntityComponentAccessor entityComponentAccessor, IEntityAllocationDatabase entityAllocationDatabase)
        {
            EntityComponentAccessor = entityComponentAccessor;
            EntityAllocationDatabase = entityAllocationDatabase;
        }

        public object Transform(object converted)
        {
            var entityData = (EntityData) converted;
            var entity = EntityAllocationDatabase.AllocateEntity(entityData.EntityId);
            EntityComponentAccessor.AddComponents(entity, entityData.Components.ToArray());
            return entityData.EntityId;
        }
    }
}