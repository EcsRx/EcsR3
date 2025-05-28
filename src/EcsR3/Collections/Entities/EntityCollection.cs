using System;
using System.Collections;
using System.Collections.Generic;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using R3;

namespace EcsR3.Collections.Entities
{
    public class EntityCollection : IEntityCollection
    {
        public IEntityAllocationDatabase EntityAllocationDatabase { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        public readonly HashSet<int> EntityLookup;

        public IReadOnlyCollection<int> Value => EntityLookup;
        
        public Observable<int> OnAdded => _onAdded;
        public Observable<int> OnRemoved => _onRemoved;
        public Observable<IReadOnlyCollection<int>> OnChanged => Observable.Merge(OnAdded, OnRemoved).Select(x => Value);
        
        private readonly Subject<int> _onAdded;
        private readonly Subject<int> _onRemoved;
        
        private readonly object _lock = new object();
        
        public EntityCollection(IEntityAllocationDatabase entityAllocationDatabase, IEntityComponentAccessor entityComponentAccessor)
        {
            EntityLookup = new HashSet<int>();
            EntityAllocationDatabase = entityAllocationDatabase;
            EntityComponentAccessor = entityComponentAccessor;

            _onAdded = new Subject<int>();
            _onRemoved = new Subject<int>();
        }
        
        public IEntity Create(int? id = null)
        {
            IEntity entity;
            lock (_lock)
            {
                if (id.HasValue && EntityLookup.Contains(id.Value))
                { throw new InvalidOperationException("id already exists"); }

                var entityId= EntityAllocationDatabase.AllocateEntity(id);
                entity = new Entity(entityId, EntityComponentAccessor);
                EntityLookup.Add(entity.Id);
            }

            _onAdded.OnNext(entity.Id);
            return entity;
        }

        public IEntity[] CreateMany(int count)
        {
            var entityIds = EntityAllocationDatabase.AllocateEntities(count);
            var entities = new IEntity[count];
            lock (_lock)
            {
                for (var i = 0; i < entities.Length; i++)
                {
                    entities[i] = new Entity(entityIds[i], EntityComponentAccessor);
                    EntityLookup.Add(entities[i].Id);
                }
            }
            return entities;
        }

        public IEntity Get(int id)
        {
            lock (_lock)
            {
                return EntityLookup.Contains(id) ? 
                    new Entity(id, EntityComponentAccessor) 
                    : null;
            }
        }

        public void Remove(int id)
        {
            var entity = Get(id);
            Remove(entity);
        }

        public void RemoveMany(IReadOnlyList<int> ids)
        {
            for(var i=0;i<ids.Count;i++)
            { Remove(ids[i]); }
        }

        protected void Remove(IEntity entity)
        {
            entity.RemoveAllComponents();
            
            lock (_lock)
            { EntityLookup.Remove(entity.Id); }
            
            EntityAllocationDatabase.ReleaseEntity(entity.Id);
            _onRemoved.OnNext(entity.Id);
        }

        public void RemoveAll()
        {
            lock (_lock)
            {
                foreach (var entityId in EntityLookup)
                { Remove(entityId); }

                EntityLookup.Clear();
            }
        }

        public void Add(IEntity entity)
        {
            lock (_lock)
            {
                if(!EntityLookup.Add(entity.Id))
                { throw new InvalidOperationException("id already exists"); }
            }
            
            _onAdded.OnNext(entity.Id);
        }

        public bool Contains(int id)
        {
            lock (_lock)
            { return EntityLookup.Contains(id); }
        }

        public IEnumerator<int> GetEnumerator()
        {
            lock (_lock)
            { return EntityLookup.GetEnumerator(); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public void Dispose()
        {
            lock (_lock)
            {
                _onAdded.Dispose();
                _onRemoved.Dispose();
                RemoveAll();
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
    }
}
