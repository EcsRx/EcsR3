using System;
using System.Collections;
using System.Collections.Generic;
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
        
        public int Create(int? id = null)
        {
            int entityId;
            lock (_lock)
            {
                if (id.HasValue && EntityLookup.Contains(id.Value))
                { throw new InvalidOperationException("id already exists"); }

                entityId= EntityAllocationDatabase.AllocateEntity(id);
                EntityLookup.Add(entityId);
            }

            _onAdded.OnNext(entityId);
            return entityId;
        }

        public int[] CreateMany(int count)
        {
            var entityIds = EntityAllocationDatabase.AllocateEntities(count);
            lock (_lock)
            {
                for (var i = 0; i < entityIds.Length; i++)
                { EntityLookup.Add(entityIds[i]); }
            }
            return entityIds;
        }

        public void RemoveMany(IReadOnlyList<int> ids)
        {
            for(var i=0;i<ids.Count;i++)
            { Remove(ids[i]); }
        }

        public void Remove(int entityId)
        {
            EntityComponentAccessor.RemoveAllComponents(entityId);
            
            lock (_lock)
            { EntityLookup.Remove(entityId); }
            
            EntityAllocationDatabase.ReleaseEntity(entityId);
            _onRemoved.OnNext(entityId);
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
