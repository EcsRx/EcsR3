using System.Linq;
using EcsR3.Plugins.Persistence.Data;
using EcsR3.Collections.Database;
using EcsR3.Collections.Entity;

namespace EcsR3.Plugins.Persistence.Transformers
{
    public class ToEntityDatabaseDataTransformer : IToEntityDatabaseDataTransformer
    {
        public IToEntityCollectionDataTransformer EntityCollectionDataTransformer { get; }

        public ToEntityDatabaseDataTransformer(IToEntityCollectionDataTransformer entityCollectionDataTransformer, IEntityCollectionFactory entityCollectionFactory)
        {
            EntityCollectionDataTransformer = entityCollectionDataTransformer;
        }

        public object Transform(object original)
        {
            var entityDatabase = (IEntityDatabase)original;

            var entityCollections = entityDatabase.Collections
                .Select(EntityCollectionDataTransformer.Transform)
                .Cast<EntityCollectionData>()
                .ToList();

            return new EntityDatabaseData
            {
                EntityCollections = entityCollections
            };
        }
    }
}