using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections.Entities;
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

        public IReadOnlyCollection<int> Value
        {
            get
            {
                lock (_lock)
                { return EnumerableEntities.ToArray(); }
            }
        }
        
        public IEnumerable<int> EnumerableEntities => CachedEntityIds;

        public Observable<IReadOnlyCollection<int>> OnChanged => Observable.Merge(OnAdded, OnRemoved).Select(x => Value);
        
        public Observable<int> OnAdded => _onEntityAdded;
        public Observable<int> OnRemoved => _onEntityRemoved;
        public Observable<int> OnRemoving => _onEntityRemoving;
        
        public IComputedEntityGroupTracker GroupTracker { get; }
        public IReadOnlyEntityCollection Collection { get; }

        private readonly Subject<int> _onEntityAdded, _onEntityRemoved, _onEntityRemoving;
        
        private readonly object _lock = new object();
        
        public ComputedEntityGroup(LookupGroup group, IComputedEntityGroupTracker tracker, IReadOnlyEntityCollection collection)
        {
            Group = group;
            Collection = collection;
            GroupTracker = tracker;
            
            _onEntityAdded = new Subject<int>();
            _onEntityRemoved = new Subject<int>();
            _onEntityRemoving = new Subject<int>();

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
            _onEntityAdded.OnNext(entityId);
        }
        
        public void OnEntityLeavingGroup(int entityId)
        {
            _onEntityRemoving.OnNext(entityId);
        }
        
        public void OnEntityLeftGroup(int entityId)
        {
            lock (_lock) { CachedEntityIds.Remove(entityId); }
            _onEntityRemoved.OnNext(entityId);
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

        public IEnumerator<int> GetEnumerator()
        { return EnumerableEntities.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}