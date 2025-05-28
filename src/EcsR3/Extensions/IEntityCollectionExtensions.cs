using System.Collections.Generic;
using EcsR3.Blueprints;
using EcsR3.Collections.Entities;
using EcsR3.Entities;

namespace EcsR3.Extensions
{
    public static class IEntityCollectionExtensions
    {
        public static void RemoveEntities(this IEntityCollection entityCollection, params IEntity[] entities)
        {
            for (var i = 0; i < entities.Length; i++)
            { entityCollection.Remove(entities[i].Id); }
        }

        public static void RemoveEntities(this IEntityCollection entityCollection, IEnumerable<IEntity> entities)
        {
            foreach (var entity in entities)
            { entityCollection.Remove(entity.Id); }
        }
        
        public static IEntity Create(this IEntityCollection entityCollection, params IBlueprint[] blueprints)
        { return Create(entityCollection, (IReadOnlyCollection<IBlueprint>)blueprints); }
        
        public static IEntity Create<T>(this IEntityCollection entityCollection) where T : IBlueprint, new()
        { return Create(entityCollection, new T()); }

        public static IEntity Create(this IEntityCollection entityCollection, IReadOnlyCollection<IBlueprint> blueprints)
        {
            var entity = entityCollection.Create();
            entity.ApplyBlueprints(blueprints);
            return entity;
        }

        public static IReadOnlyList<IEntity> CreateMany(this IEntityCollection entityCollection, int count, params IBlueprint[] blueprints)
        { return CreateMany(entityCollection, count, (IReadOnlyCollection<IBlueprint>)blueprints); }
        
        public static IReadOnlyList<IEntity> CreateMany<T>(this IEntityCollection entityCollection, int count) where T : IBlueprint, new()
        { return CreateMany(entityCollection, count, new T()); }
        
        public static IReadOnlyList<IEntity> CreateMany(this IEntityCollection entityCollection, int count, IReadOnlyCollection<IBlueprint> blueprints)
        {
            var entities = entityCollection.CreateMany(count);
            for (var i = 0; i < count; i++)
            {
                var entity = entities[i];
                entity.ApplyBlueprints(blueprints);
            }
            return entities;
        }
    }
}