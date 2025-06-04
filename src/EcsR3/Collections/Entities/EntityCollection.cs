using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using R3;

namespace EcsR3.Collections.Entities
{
    public class EntityCollection : IEntityCollection
    {
        public IEntityAllocationDatabase EntityAllocationDatabase { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        public readonly HashSet<Entity> EntityLookup;

        public IReadOnlyCollection<Entity> Value => EntityLookup;
        
        public Observable<Entity> OnAdded => _onAdded;
        public Observable<Entity> OnRemoved => _onRemoved;
        public Observable<IReadOnlyCollection<Entity>> OnChanged => Observable.Merge(OnAdded, OnRemoved).Select(x => Value);
        
        private readonly Subject<Entity> _onAdded;
        private readonly Subject<Entity> _onRemoved;
        
        private readonly object _lock = new object();
        
        public EntityCollection(IEntityAllocationDatabase entityAllocationDatabase, IEntityComponentAccessor entityComponentAccessor)
        {
            EntityLookup = new HashSet<Entity>();
            EntityAllocationDatabase = entityAllocationDatabase;
            EntityComponentAccessor = entityComponentAccessor;

            _onAdded = new Subject<Entity>();
            _onRemoved = new Subject<Entity>();
        }
        
        public Entity Create(int? id = null)
        {
            var entity = EntityAllocationDatabase.AllocateEntity(id);
            lock (_lock) { EntityLookup.Add(entity); }
            _onAdded.OnNext(entity);
            return entity;
        }

        public Entity[] CreateMany(int count)
        {
            var entityIds = EntityAllocationDatabase.AllocateEntities(count);
            lock (_lock)
            {
                for (var i = 0; i < entityIds.Length; i++)
                { EntityLookup.Add(entityIds[i]); }
            }
            return entityIds;
        }

        public void Remove(IReadOnlyList<Entity> entities)
        {
            for(var i=0;i<entities.Count;i++)
            { Remove(entities[i]); }
        }

        public void Remove(Entity entity)
        {
            EntityComponentAccessor.RemoveAllComponents(entity);
            
            lock (_lock)
            { EntityLookup.Remove(entity); }
            
            EntityAllocationDatabase.ReleaseEntity(entity);
            _onRemoved.OnNext(entity);
        }

        public void RemoveAll()
        {
            Entity[] entities;
            lock (_lock)
            { entities = EntityLookup.ToArray(); }

            foreach (var entity in entities)
            { Remove(entity); }

            lock (_lock)
            { EntityLookup.Clear(); }
        }

        public bool Contains(Entity entity)
        {
            lock (_lock)
            { return EntityLookup.Contains(entity); }
        }

        public IEnumerator<Entity> GetEnumerator()
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
