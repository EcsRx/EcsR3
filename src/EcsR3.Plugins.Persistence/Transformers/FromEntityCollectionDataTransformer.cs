using System.Linq;
using EcsR3.Collections.Entities;
using EcsR3.Plugins.Persistence.Data;
using SystemsR3.Extensions;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;

namespace EcsR3.Plugins.Persistence.Transformers
{
    public class FromEntityCollectionDataTransformer : IFromEntityCollectionDataTransformer
    {
        public IFromEntityDataTransformer EntityDataTransformer { get; }
        public IEntityAllocationDatabase EntityAllocationDatabase { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        public FromEntityCollectionDataTransformer(IFromEntityDataTransformer entityDataTransformer, IEntityAllocationDatabase entityAllocationDatabase, IEntityComponentAccessor entityComponentAccessor)
        {
            EntityDataTransformer = entityDataTransformer;
            EntityAllocationDatabase = entityAllocationDatabase;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public object Transform(object converted)
        {
            var collectionData = (EntityCollectionData) converted;
            var collection = new EntityCollection(EntityAllocationDatabase, EntityComponentAccessor);
            var entities = collectionData.Entities
                .Select(EntityDataTransformer.Transform)
                .Cast<int>();
            
            entities.ForEachRun(x => collection.Create(x));
            return collection;
        }
    }
}