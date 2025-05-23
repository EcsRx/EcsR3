using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Blueprints;
using EcsR3.Collections.Entity;
using EcsR3.Components;
using EcsR3.Entities;

namespace EcsR3.Extensions
{
    public static class IEntityCollectionExtensions
    {
        public static void RemoveEntitiesContaining<T>(this IEntityCollection entityCollection)
            where T : class, IComponent
        {
            var entities = entityCollection
                .Where(entity => entity.HasComponent<T>())
                .ToArray();
            
            for(var i=0;i<entities.Length;i++)
            { entityCollection.Remove(entities[i].Id); }
        }

        public static void RemoveEntitiesContaining(this IEntityCollection entityCollection, params Type[] components)
        {
            var entities = entityCollection
                .Where(entity => components.Any(x => entity.HasAllComponents(x)))
                .ToArray();

            for (var i = 0; i < entities.Length; i++)
            { entityCollection.Remove(entities[i].Id); }
        }
        
        public static void RemoveEntities(this IEntityCollection entityCollection, Func<IEntity, bool> predicate)
        {
            var entities = entityCollection.Where(predicate).ToArray();
            for (var i = 0; i < entities.Length; i++)
            { entityCollection.Remove(entities[i].Id); }
        }

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

        public static IEnumerable<IEntity> Query(this IEntityCollection entityCollection, IEntityCollectionQuery query)
        { return query.Execute(entityCollection); }

        public static IEntity Create(this IEntityCollection entityCollection, params IBlueprint[] blueprints)
        { return Create(entityCollection, (IEnumerable<IBlueprint>)blueprints); }
        
        public static IEntity Create<T>(this IEntityCollection entityCollection) where T : IBlueprint, new()
        { return Create(entityCollection, new T()); }

        public static IEntity Create(this IEntityCollection entityCollection, IEnumerable<IBlueprint> blueprints)
        {
            var entity = entityCollection.Create();
            entity.ApplyBlueprints(blueprints);
            return entity;
        }
    }
}