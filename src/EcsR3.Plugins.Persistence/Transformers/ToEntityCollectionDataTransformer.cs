using System.Linq;
using EcsR3.Collections.Entities;
using EcsR3.Plugins.Persistence.Data;

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
                .Select(x => EntityDataTransformer.Transform(x))
                .Cast<EntityData>()
                .ToList();

            return new EntityCollectionData
            {
                Entities = entityData
            };
        }

    }
}