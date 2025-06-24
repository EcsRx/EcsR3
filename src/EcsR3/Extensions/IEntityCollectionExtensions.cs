using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Blueprints;
using EcsR3.Collections.Entities;
using EcsR3.Components;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;

namespace EcsR3.Extensions
{
    public static class IEntityCollectionExtensions
    {
        public static Entity Create(this IEntityCollection entityCollection, IEntityComponentAccessor accessor, params IBlueprint[] blueprints)
        { return Create(entityCollection, accessor, (IReadOnlyCollection<IBlueprint>)blueprints); }
        
        public static Entity Create<T>(this IEntityCollection entityCollection, IEntityComponentAccessor accessor) where T : IBlueprint, new()
        { return Create(entityCollection, accessor, new T()); }

        public static Entity Create(this IEntityCollection entityCollection, IEntityComponentAccessor accessor, IReadOnlyCollection<IBlueprint> blueprints)
        {
            var entity = entityCollection.Create();
            foreach(var blueprint in blueprints)
            { blueprint.Apply(accessor, entity); }
            return entity;
        }
        
        public static IReadOnlyList<Entity> CreateMany(this IEntityCollection entityCollection,  IEntityComponentAccessor accessor, int count, params IBatchedBlueprint[] blueprints)
        { return CreateMany(entityCollection, accessor, count, (IReadOnlyCollection<IBatchedBlueprint>)blueprints); }
        
        public static IReadOnlyList<Entity> CreateMany<T>(this IEntityCollection entityCollection,  IEntityComponentAccessor accessor, int count) where T : IBatchedBlueprint, new()
        { return CreateMany(entityCollection, accessor, count, new T()); }
        
        public static IReadOnlyList<Entity> CreateMany(this IEntityCollection entityCollection, IEntityComponentAccessor accessor, int count, IReadOnlyCollection<IBatchedBlueprint> blueprints)
        {
            var entities = entityCollection.CreateMany(count);
            foreach (var blueprint in blueprints)
            { blueprint.Apply(accessor, entities); }
            return entities;
        }
        
        public static void RemoveEntitiesContaining<T>(this IEntityCollection entityCollection, IEntityComponentAccessor entityComponentAccessor)
            where T : class, IComponent
        {
            var entities = entityCollection
                .Where(entityComponentAccessor.HasComponent<T>)
                .ToArray();
            
            entityCollection.Remove(entities);
        }

        public static void RemoveEntitiesContaining(this IEntityCollection entityCollection, IEntityComponentAccessor entityComponentAccessor, params Type[] components)
        {
            var entities = entityCollection
                .Where(entity => entityComponentAccessor.HasAnyComponents(entity, components))
                .ToArray();

            entityCollection.Remove(entities);
        }
        
        public static void Remove(this IEntityCollection entityCollection, IEntityComponentAccessor entityComponentAccessor, Func<IEntityComponentAccessor, Entity, bool> predicate)
        {
            var entities = entityCollection
                .Where(x => predicate(entityComponentAccessor, x))
                .ToArray();

            entityCollection.Remove(entities);
        }

        public static void Remove(this IEntityCollection entityCollection, params Entity[] entities)
        { entityCollection.Remove(entities); }

        /// <summary>
        /// Gets an enumerable collection of entities for you to iterate through, from the collection provided
        /// This is not cached and will always query the live data.
        /// </summary>
        /// <remarks>
        /// So in most cases an IComputedEntityGroup is a better option to use for repeat queries as it internally
        /// will update a maintained list of entities without having to enumerate the entire collection/s.
        /// </remarks>
        /// <param name="collection">The collection to operate on</param>
        /// <param name="entityComponentAccessor">The component accessor</param>
        /// <param name="group">The group to match entities on</param>
        /// <returns>An enumerable to access the data inside the collection/s</returns>
        public static IEnumerable<Entity> GetEntitiesMatching(this IEntityCollection collection, IEntityComponentAccessor entityComponentAccessor, IGroup group)
        {
            if(group is EmptyGroup)
            { return Array.Empty<Entity>(); }
            
            return collection.Value.MatchingGroup(entityComponentAccessor, group);
        }
        
        /// <summary>
        /// Removes all entities matching the given group
        /// </summary>
        /// <param name="entityCollection">The collection to operate on</param>
        /// <param name="entityComponentAccessor">The component accessor</param>
        /// <param name="group">The group to match against</param>
        public static void RemoveEntitiesMatching(this IEntityCollection entityCollection, IEntityComponentAccessor entityComponentAccessor, IGroup group)
        {
            var entities = entityCollection
                .Where(entity => entityComponentAccessor.MatchesGroup(entity, group))
                .ToArray();

            entityCollection.Remove(entities);
        }
    }
}