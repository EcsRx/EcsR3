using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections.Entity;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Groups.Tracking.Trackers;
using R3;
using SystemsR3.Extensions;

namespace EcsR3.Computeds.Entities
{
    public class ComputedEntityGroup : IComputedEntityGroup
    {
        public LookupGroup Group { get; }
        
        public readonly HashSet<int> CachedEntityIds;
        public readonly CompositeDisposable Subscriptions;

        public IReadOnlyCollection<IEntity> Value
        {
            get
            {
                lock (_lock)
                { return EnumerableEntities.ToArray(); }
            }
        }
        
        public IEnumerable<IEntity> EnumerableEntities => CachedEntityIds.Select(Collection.Get);

        public Observable<IReadOnlyCollection<IEntity>> OnChanged => Observable.Merge(OnAdded, OnRemoved).Select(x => Value);
        
        public Observable<IEntity> OnAdded => _onEntityAdded;
        public Observable<IEntity> OnRemoved => _onEntityRemoved;
        public Observable<IEntity> OnRemoving => _onEntityRemoving;
        
        public IObservableGroupTracker GroupTracker { get; }
        public IReadOnlyEntityCollection Collection { get; }

        private readonly Subject<IEntity> _onEntityAdded, _onEntityRemoved, _onEntityRemoving;
        
        private readonly object _lock = new object();
        
        public ComputedEntityGroup(LookupGroup group, IObservableGroupTracker tracker, IReadOnlyEntityCollection collection)
        {
            Group = group;
            Collection = collection;
            GroupTracker = tracker;
            
            _onEntityAdded = new Subject<IEntity>();
            _onEntityRemoved = new Subject<IEntity>();
            _onEntityRemoving = new Subject<IEntity>();

            Subscriptions = new CompositeDisposable();
            CachedEntityIds = new HashSet<int>();

            GroupTracker.OnEntityJoinedGroup
                .Subscribe(OnEntityJoinedGroup)
                .AddTo(Subscriptions);
            
            GroupTracker.OnEntityLeavingGroup
                .Subscribe(OnEntityLeavingGroup)
                .AddTo(Subscriptions);
            
            GroupTracker.OnEntityLeftGroup
                .Subscribe(OnEntityLeftGroup)
                .AddTo(Subscriptions);

            CachedEntityIds = new HashSet<int>(GroupTracker.GetMatchedEntityIds());
            
        }

        public void OnEntityJoinedGroup(int entityId)
        {
            lock (_lock) { CachedEntityIds.Add(entityId); }
            _onEntityAdded.OnNext(Collection.Get(entityId));
        }
        
        public void OnEntityLeavingGroup(int entityId)
        {
            var entity = Collection.Get(entityId);
            _onEntityRemoving.OnNext(entity);
        }
        
        public void OnEntityLeftGroup(int entityId)
        {
            var entity = Collection.Get(entityId);
            lock (_lock) { CachedEntityIds.Remove(entityId); }
            _onEntityRemoved.OnNext(entity);
        }

        public bool Contains(int id)
        {
            lock(_lock)
            { return CachedEntityIds.Contains(id); }
        }

        public IEntity Get(int id)
        {
            lock (_lock)
            { return CachedEntityIds.TryGetValue(id, out var entityId) ? Collection.Get(entityId) : null; }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                Subscriptions.DisposeAll();
                _onEntityAdded.Dispose();
                _onEntityRemoved.Dispose();
                _onEntityRemoving.Dispose();
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                { return CachedEntityIds.Count; }
            }
        }

        public IEnumerator<IEntity> GetEnumerator()
        { return EnumerableEntities.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}