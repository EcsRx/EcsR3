using System.Linq;
using EcsR3.Plugins.Persistence.Data;
using EcsR3.Collections.Entity;

namespace EcsR3.Plugins.Persistence.Transformers
{
    public class ToEntityCollectionDataTransformer : IToEntityCollectionDataTransformer
    {
        public IToEntityDataTransformer EntityDataTransformer { get; }

        public ToEntityCollectionDataTransformer(IToEntityDataTransformer entityDataTransformer)
        {
            EntityDataTransformer = entityDataTransformer;
        }

        public object Transform(object original)
        {
            var collection = (IEntityCollection)original;

            var entityData = collection
                .Select(EntityDataTransformer.Transform)
                .Cast<EntityData>()
                .ToList();

            return new EntityCollectionData
            {
                CollectionId = collection.Id,
                Entities = entityData
            };
        }

    }
}