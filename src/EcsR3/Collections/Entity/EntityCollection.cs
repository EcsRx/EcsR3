using System;
using System.Collections;
using System.Collections.Generic;
using EcsR3.Entities;
using R3;

namespace EcsR3.Collections.Entity
{
    public class EntityCollection : IEntityCollection
    {
        public IEntityFactory EntityFactory { get; }
        
        public readonly Dictionary<int, IEntity> EntityLookup;

        public IReadOnlyCollection<IEntity> Value => EntityLookup.Values;
        
        public Observable<IEntity> OnAdded => _onAdded;
        public Observable<IEntity> OnRemoved => _onRemoved;
        public Observable<IReadOnlyCollection<IEntity>> OnChanged => Observable.Merge(OnAdded, OnRemoved).Select(x => Value);
        
        private readonly Subject<IEntity> _onAdded;
        private readonly Subject<IEntity> _onRemoved;
        
        private readonly object _lock = new object();
        
        public EntityCollection(IEntityFactory entityFactory)
        {
            EntityLookup = new Dictionary<int, IEntity>();
            EntityFactory = entityFactory;

            _onAdded = new Subject<IEntity>();
            _onRemoved = new Subject<IEntity>();
        }
        
        public IEntity Create(int? id = null)
        {
            IEntity entity;
            lock (_lock)
            {
                if (id.HasValue && EntityLookup.ContainsKey(id.Value))
                { throw new InvalidOperationException("id already exists"); }

                entity = EntityFactory.Create(id);
                EntityLookup.Add(entity.Id, entity);
            }

            _onAdded.OnNext(entity);
            return entity;
        }

        public IEntity Get(int id)
        {
            lock(_lock)
            { return EntityLookup[id]; }
        }

        public void Remove(int id)
        {
            var entity = Get(id);
            Remove(entity);
        }

        protected void Remove(IEntity entity)
        {
            entity.RemoveAllComponents();
            
            lock (_lock)
            { EntityLookup.Remove(entity.Id); }
            
            EntityFactory.Destroy(entity.Id);
            _onRemoved.OnNext(entity);
        }

        public void RemoveAll()
        {
            lock (_lock)
            {
                foreach (var entity in EntityLookup.Values)
                { Remove(entity); }

                EntityLookup.Clear();
            }
        }

        public void Add(IEntity entity)
        {
            lock (_lock)
            {
                if (!EntityLookup.TryAdd(entity.Id, entity))
                { throw new InvalidOperationException("id already exists"); }
            }
            
            _onAdded.OnNext(entity);
        }

        public bool Contains(int id)
        {
            lock (_lock)
            { return EntityLookup.ContainsKey(id); }
        }

        public IEnumerator<IEntity> GetEnumerator()
        {
            lock (_lock)
            { return EntityLookup.Values.GetEnumerator(); }
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
