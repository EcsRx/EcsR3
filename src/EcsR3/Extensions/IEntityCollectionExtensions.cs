using System.Collections.Generic;
using EcsR3.Blueprints;
using EcsR3.Collections.Entities;
using EcsR3.Entities.Accessors;

namespace EcsR3.Extensions
{
    public static class IEntityCollectionExtensions
    {
        public static int Create(this IEntityCollection entityCollection, IEntityComponentAccessor accessor, params IBlueprint[] blueprints)
        { return Create(entityCollection, accessor, (IReadOnlyCollection<IBlueprint>)blueprints); }
        
        public static int Create<T>(this IEntityCollection entityCollection, IEntityComponentAccessor accessor) where T : IBlueprint, new()
        { return Create(entityCollection, accessor, new T()); }

        public static int Create(this IEntityCollection entityCollection, IEntityComponentAccessor accessor, IReadOnlyCollection<IBlueprint> blueprints)
        {
            var entity = entityCollection.Create();
            foreach(var blueprint in blueprints)
            { blueprint.Apply(accessor, entity); }
            return entity;
        }

        public static IReadOnlyList<int> CreateMany(this IEntityCollection entityCollection,  IEntityComponentAccessor accessor, int count, params IBlueprint[] blueprints)
        { return CreateMany(entityCollection, accessor, count, (IReadOnlyCollection<IBlueprint>)blueprints); }
        
        public static IReadOnlyList<int> CreateMany<T>(this IEntityCollection entityCollection,  IEntityComponentAccessor accessor, int count) where T : IBlueprint, new()
        { return CreateMany(entityCollection, accessor, count, new T()); }
        
        public static IReadOnlyList<int> CreateMany(this IEntityCollection entityCollection, IEntityComponentAccessor accessor, int count, IReadOnlyCollection<IBlueprint> blueprints)
        {
            var entities = entityCollection.CreateMany(count);
            foreach (var blueprint in blueprints)
            { blueprint.ApplyToAll(accessor, entities); }
            return entities;
        }
    }
}