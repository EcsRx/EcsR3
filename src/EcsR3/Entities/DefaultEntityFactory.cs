﻿using System;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities.Routing;
using SystemsR3.Pools;

namespace EcsR3.Entities
{
    public class DefaultEntityFactory : IEntityFactory
    {
        public IIdPool IdPool { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IEntityChangeRouter EntityChangeRouter { get; }

        public DefaultEntityFactory(IIdPool idPool, IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IEntityChangeRouter entityChangeRouter)
        {
            IdPool = idPool;
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
            EntityChangeRouter = entityChangeRouter;
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
            return new Entity(usedId, ComponentDatabase, ComponentTypeLookup, EntityChangeRouter);
        }

        public void Destroy(int entityId)
        { IdPool.ReleaseInstance(entityId); }
    }
}