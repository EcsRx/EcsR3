using System.Linq;
using EcsR3.Plugins.Persistence.Data;
using SystemsR3.Extensions;
using EcsR3.Collections.Entity;
using EcsR3.Entities;
using EcsR3.Extensions;

namespace EcsR3.Plugins.Persistence.Transformers
{
    public class FromEntityCollectionDataTransformer : IFromEntityCollectionDataTransformer
    {
        public IFromEntityDataTransformer EntityDataTransformer { get; }
        public IEntityCollectionFactory EntityCollectionFactory { get; }

        public FromEntityCollectionDataTransformer(IFromEntityDataTransformer entityDataTransformer, IEntityCollectionFactory entityCollectionFactory)
        {
            EntityDataTransformer = entityDataTransformer;
            EntityCollectionFactory = entityCollectionFactory;
        }
        
        public object Transform(object converted)
        {
            var collectionData = (EntityCollectionData) converted;
            var collection = EntityCollectionFactory.Create(collectionData.CollectionId);
            var entities = collectionData.Entities
                .Select(EntityDataTransformer.Transform)
                .Cast<IEntity>();
            
            entities.ForEachRun(collection.AddEntity);
            return collection;
        }
    }
}