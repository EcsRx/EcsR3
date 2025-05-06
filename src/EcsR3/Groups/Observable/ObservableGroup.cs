using System;
using System.Collections;
using System.Collections.Generic;
using EcsR3.Collections.Entity;
using EcsR3.Entities;
using EcsR3.Groups.Observable.Tracking.Trackers;
using EcsR3.Lookups;
using SystemsR3.Extensions;
using R3;

namespace EcsR3.Groups.Observable
{
    public class ObservableGroup : IObservableGroup
    {
        public LookupGroup Group { get; }
        
        public readonly EntityLookup CachedEntities;
        public readonly List<IDisposable> Subscriptions;

        public Observable<IEntity> OnEntityAdded => _onEntityAdded;
        public Observable<IEntity> OnEntityRemoved => _onEntityRemoved;
        public Observable<IEntity> OnEntityRemoving => _onEntityRemoving;
        public IObservableGroupTracker GroupTracker { get; }
        public IEntityCollection Collection { get; }

        private readonly Subject<IEntity> _onEntityAdded;
        private readonly Subject<IEntity> _onEntityRemoved;
        private readonly Subject<IEntity> _onEntityRemoving;
        
        private readonly object _lock = new object();
        
        public ObservableGroup(LookupGroup group, IObservableGroupTracker tracker, IEntityCollection collection)
        {
            Group = group;
            Collection = collection;
            GroupTracker = tracker;
            
            _onEntityAdded = new Subject<IEntity>();
            _onEntityRemoved = new Subject<IEntity>();
            _onEntityRemoving = new Subject<IEntity>();

            Subscriptions = new List<IDisposable>();
            CachedEntities = new EntityLookup();

            GroupTracker.OnEntityJoinedGroup
                .Subscribe(OnEntityJoinedGroup)
                .AddTo(Subscriptions);

            foreach (var entityId in GroupTracker.GetMatchedEntityIds())
            {
                var entity = collection.GetEntity(entityId);
                CachedEntities.Add(entity);
            }
        }

        public void OnEntityJoinedGroup(int entityId)
        {
            var entity = Collection.GetEntity(entityId);
            lock (_lock) { CachedEntities.Add(entity); }
            _onEntityAdded.OnNext(entity);
        }
        
        public void OnEntityLeavingGroup(int entityId)
        {
            var entity = Collection.GetEntity(entityId);
            _onEntityRemoving.OnNext(entity);
        }
        
        public void OnEntityLeftGroup(int entityId)
        {
            var entity = Collection.GetEntity(entityId);
            lock (_lock) { CachedEntities.Remove(entityId); }
            _onEntityRemoved.OnNext(entity);
        }

        public bool ContainsEntity(int id)
        {
            lock(_lock)
            { return CachedEntities.Contains(id); }
        }

        public IEntity GetEntity(int id)
        {
            lock (_lock)
            { return CachedEntities[id]; }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                Subscriptions.DisposeAll();
                GroupTracker.Dispose();
                _onEntityAdded.Dispose();
                _onEntityRemoved.Dispose();
                _onEntityRemoving.Dispose();
            }
        }

        public IEnumerator<IEntity> GetEnumerator()
        { return CachedEntities.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public int Count
        {
            get
            {
                lock (_lock)
                { return CachedEntities.Count; }
            }
        }

        public IEntity this[int index]
        {
            get
            {
                lock (_lock)
                { return CachedEntities.GetByIndex(index); }
            }
        }
    }
}