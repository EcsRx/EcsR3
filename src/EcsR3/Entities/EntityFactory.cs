using System;
using System.Collections.Generic;
using EcsR3.Entities.Accessors;
using SystemsR3.Pools;

namespace EcsR3.Entities
{
    public class EntityFactory : IEntityFactory
    {
        public IIdPool IdPool { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        public EntityFactory(IIdPool idPool, IEntityComponentAccessor entityComponentAccessor)
        {
            IdPool = idPool;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public int GetId(int? id = null)
        {
            if(!id.HasValue)
            { return IdPool.AllocateInstance(); }

            IdPool.AllocateSpecificId(id.Value);
            return id.Value;
        }
        
        public IEntity Create(int? id = null)
        {
            if(id.HasValue && id.Value == 0)
            { throw new ArgumentException("id must be null or > 0"); }
            
            var usedId = GetId(id);
            return new Entity(usedId, EntityComponentAccessor);
        }

        public void Destroy(int entityId)
        { IdPool.ReleaseInstance(entityId); }

        public IReadOnlyList<IEntity> CreateMany(int count)
        {
            var ids = IdPool.AllocateMany(count);
            var entities = new IEntity[count];
            for (var i = 0; i < count; i++)
            { entities[i] = new Entity(ids[i], EntityComponentAccessor); }
            return entities;
        }
    }
}