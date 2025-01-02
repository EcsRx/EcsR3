using System.Linq;
using EcsR3.Plugins.Persistence.Data;
using SystemsR3.Extensions;
using EcsR3.Collections.Database;
using EcsR3.Collections.Entity;

namespace EcsR3.Plugins.Persistence.Transformers
{
    public class FromEntityDatabaseDataTransformer : IFromEntityDatabaseDataTransformer
    {
        public IFromEntityCollectionDataTransformer EntityCollectionDataTransformer { get; }
        public IEntityCollectionFactory EntityCollectionFactory { get; }

        public FromEntityDatabaseDataTransformer(IFromEntityCollectionDataTransformer entityCollectionDataTransformer, IEntityCollectionFactory entityCollectionFactory)
        {
            EntityCollectionDataTransformer = entityCollectionDataTransformer;
            EntityCollectionFactory = entityCollectionFactory;
        }

        public object Transform(object converted)
        {
            var entityDatabaseData = (EntityDatabaseData) converted;
            var entityDatabase = new EntityDatabase(EntityCollectionFactory);
            entityDatabaseData.EntityCollections
                .Select(EntityCollectionDataTransformer.Transform)
                .Cast<IEntityCollection>()
                .ForEachRun(x =>
                {
                    if (entityDatabase.Collections.Any(e => e.Id == x.Id))
                    {
                        entityDatabase.RemoveCollection(x.Id);
                        
                    }
                    entityDatabase.AddCollection(x);
                });

            return entityDatabase;
        }
    }
}