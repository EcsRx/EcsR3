using System.Collections;
using System.Collections.Generic;
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
        
        public readonly HashSet<Entity> CachedEntities;
        public readonly CompositeDisposable Subscriptions;

        public IReadOnlyCollection<Entity> Value => CachedEntities;

        public Observable<IReadOnlyCollection<Entity>> OnChanged => Observable.Merge(OnAdded, OnRemoved).Select(x => Value);
        public Observable<Unit> OnHasChanged => Observable.Merge(OnAdded, OnRemoved).Select(x => Unit.Default);
        
        public Observable<Entity> OnAdded => _onEntityAdded;
        public Observable<Entity> OnRemoved => _onEntityRemoved;
        public Observable<Entity> OnRemoving => _onEntityRemoving;
        
        public IComputedEntityGroupTracker GroupTracker { get; }
        public IReadOnlyEntityCollection Collection { get; }

        private readonly Subject<Entity> _onEntityAdded, _onEntityRemoved, _onEntityRemoving;
        
        private readonly object _lock = new object();
        
        public ComputedEntityGroup(LookupGroup group, IComputedEntityGroupTracker tracker, IReadOnlyEntityCollection collection)
        {
            Group = group;
            Collection = collection;
            GroupTracker = tracker;
            
            _onEntityAdded = new Subject<Entity>();
            _onEntityRemoved = new Subject<Entity>();
            _onEntityRemoving = new Subject<Entity>();

            Subscriptions = new CompositeDisposable();
            CachedEntities = new HashSet<Entity>();

            GroupTracker.OnEntityJoinedGroup
                .Subscribe(OnEntityJoinedGroup)
                .AddTo(Subscriptions);
            
            GroupTracker.OnEntityLeavingGroup
                .Subscribe(OnEntityLeavingGroup)
                .AddTo(Subscriptions);
            
            GroupTracker.OnEntityLeftGroup
                .Subscribe(OnEntityLeftGroup)
                .AddTo(Subscriptions);

            CachedEntities = new HashSet<Entity>(GroupTracker.GetMatchedEntities());
        }

        public void OnEntityJoinedGroup(Entity entity)
        {
            lock (_lock) { CachedEntities.Add(entity); }
            _onEntityAdded.OnNext(entity);
        }
        
        public void OnEntityLeavingGroup(Entity entity)
        {
            _onEntityRemoving.OnNext(entity);
        }
        
        public void OnEntityLeftGroup(Entity entity)
        {
            lock (_lock) { CachedEntities.Remove(entity); }
            _onEntityRemoved.OnNext(entity);
        }

        public bool Contains(Entity entity)
        {
            lock(_lock)
            { return CachedEntities.Contains(entity); }
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
                { return CachedEntities.Count; }
            }
        }

        public IEnumerator<Entity> GetEnumerator()
        { return CachedEntities.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}