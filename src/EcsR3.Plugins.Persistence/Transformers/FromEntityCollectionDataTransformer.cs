using System.Linq;
using EcsR3.Plugins.Persistence.Data;
using SystemsR3.Extensions;
using EcsR3.Collections.Entity;
using EcsR3.Entities;

namespace EcsR3.Plugins.Persistence.Transformers
{
    public class FromEntityCollectionDataTransformer : IFromEntityCollectionDataTransformer
    {
        public IFromEntityDataTransformer EntityDataTransformer { get; }
        public IEntityFactory EntityFactory { get; }

        public FromEntityCollectionDataTransformer(IFromEntityDataTransformer entityDataTransformer, IEntityFactory entityFactory)
        {
            EntityDataTransformer = entityDataTransformer;
            EntityFactory = entityFactory;
        }
        
        public object Transform(object converted)
        {
            var collectionData = (EntityCollectionData) converted;
            var collection = new EntityCollection(EntityFactory);
            var entities = collectionData.Entities
                .Select(EntityDataTransformer.Transform)
                .Cast<IEntity>();
            
            entities.ForEachRun(collection.AddEntity);
            return collection;
        }
    }
}