using System;
using System.Collections;
using System.Collections.Generic;
using EcsR3.Blueprints;
using EcsR3.Collections.Events;
using EcsR3.Entities;
using EcsR3.Lookups;
using R3;

namespace EcsR3.Collections.Entity
{
    public class EntityCollection : IEntityCollection, IDisposable
    {
        public IEntityFactory EntityFactory { get; }
        
        public readonly EntityLookup EntityLookup;

        public Observable<CollectionEntityEvent> EntityAdded => _onEntityAdded;
        public Observable<CollectionEntityEvent> EntityRemoved => _onEntityRemoved;
        
        private readonly Subject<CollectionEntityEvent> _onEntityAdded;
        private readonly Subject<CollectionEntityEvent> _onEntityRemoved;
        
        private readonly object _lock = new object();
        
        public EntityCollection(IEntityFactory entityFactory)
        {
            EntityLookup = new EntityLookup();
            EntityFactory = entityFactory;

            _onEntityAdded = new Subject<CollectionEntityEvent>();
            _onEntityRemoved = new Subject<CollectionEntityEvent>();
        }
        
        public IEntity CreateEntity(IBlueprint blueprint = null, int? id = null)
        {
            IEntity entity;
            lock (_lock)
            {
                if (id.HasValue && EntityLookup.Contains(id.Value))
                { throw new InvalidOperationException("id already exists"); }

                entity = EntityFactory.Create(id);

                EntityLookup.Add(entity);
            }

            _onEntityAdded.OnNext(new CollectionEntityEvent(entity));
            blueprint?.Apply(entity);
            
            return entity;
        }

        public IEntity GetEntity(int id)
        {
            lock(_lock)
            { return EntityLookup[id]; }
        }

        public void RemoveEntity(int id)
        {
            var entity = GetEntity(id);
            RemoveEntity(entity);
        }

        protected void RemoveEntity(IEntity entity)
        {
            entity.RemoveAllComponents();
            
            lock (_lock)
            { EntityLookup.Remove(entity.Id); }
            
            EntityFactory.Destroy(entity.Id);
            _onEntityRemoved.OnNext(new CollectionEntityEvent(entity));
        }

        public void RemoveAllEntities()
        {
            lock (_lock)
            {
                for (var i = EntityLookup.Count - 1; i >= 0; i--)
                { RemoveEntity(EntityLookup.GetByIndex(i)); }
            }
        }

        public void AddEntity(IEntity entity)
        {
            lock (_lock)
            { EntityLookup.Add(entity); }
            
            _onEntityAdded.OnNext(new CollectionEntityEvent(entity));
        }

        public bool ContainsEntity(int id)
        {
            lock (_lock)
            { return EntityLookup.Contains(id); }
        }

        public IEnumerator<IEntity> GetEnumerator()
        { return EntityLookup.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public void Dispose()
        {
            lock (_lock)
            {
                _onEntityAdded.Dispose();
                _onEntityRemoved.Dispose();
                EntityLookup.Clear();
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                { return EntityLookup.Count; }
            }
        }

        public IEntity this[int index]
        {
            get
            {
                lock (_lock)
                { return EntityLookup.GetByIndex(index); }
            }
        }
    }
}
