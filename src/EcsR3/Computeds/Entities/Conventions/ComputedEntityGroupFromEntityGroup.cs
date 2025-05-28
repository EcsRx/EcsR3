using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using R3;
using SystemsR3.Extensions;

namespace EcsR3.Computeds.Entities.Conventions
{
    public abstract class ComputedEntityGroupFromEntityGroup : IComputedEntityGroup
    {
        public LookupGroup Group { get; }
        
        public readonly HashSet<int> CachedEntityIds;
        public readonly CompositeDisposable Subscriptions;
        public readonly IComputedEntityGroup DataSource;
        public readonly IEntityComponentAccessor EntityComponentAccessor;

        public IReadOnlyCollection<int> Value
        {
            get
            {
                lock (_lock)
                { return CachedEntityIds; }
            }
        }

        public Observable<IReadOnlyCollection<int>> OnChanged => Observable.Merge(OnAdded, OnRemoved).Select(x => Value);
        
        public Observable<int> OnAdded => _onEntityAdded;
        public Observable<int> OnRemoved => _onEntityRemoved;
        public Observable<int> OnRemoving => _onEntityRemoving;
        
        private readonly Subject<int> _onEntityAdded, _onEntityRemoved, _onEntityRemoving;
        private readonly object _lock = new object();
        
        public ComputedEntityGroupFromEntityGroup(IEntityComponentAccessor entityComponentAccessor, IComputedEntityGroup dataSource)
        {
            Group = dataSource.Group;
            DataSource = dataSource;
            EntityComponentAccessor = entityComponentAccessor;

            _onEntityAdded = new Subject<int>();
            _onEntityRemoved = new Subject<int>();
            _onEntityRemoving = new Subject<int>();

            Subscriptions = new CompositeDisposable();
            CachedEntityIds = new HashSet<int>(dataSource.Where(IsEntityValid));

            ListenForChanges();
        }

        protected void ListenForChanges()
        {
            DataSource.OnAdded.Subscribe(OnEntityAddedToGroup).AddTo(Subscriptions);
            DataSource.OnRemoving.Subscribe(_onEntityRemoving.OnNext).AddTo(Subscriptions);
            DataSource.OnRemoved.Subscribe(_onEntityRemoved.OnNext).AddTo(Subscriptions);
            
            RefreshWhen().Subscribe(_ => Refresh()).AddTo(Subscriptions);
        }

        public abstract bool IsEntityValid(int entity);
        public virtual Observable<Unit> RefreshWhen() => Observable.Never<Unit>();
        
        public virtual void Refresh()
        {
            foreach (var entityId in DataSource)
            {
                var isValid = IsEntityValid(entityId);
                var containsEntity = Contains(entityId);

                if (isValid)
                {
                    if(containsEntity) { continue; }

                    lock (_lock)
                    { CachedEntityIds.Add(entityId); }

                    _onEntityAdded.OnNext(entityId);
                    continue;
                }

                if (!containsEntity) { continue; }
                
                lock (_lock)
                { CachedEntityIds.Remove(entityId); }
                
                _onEntityRemoving.OnNext(entityId);
                _onEntityRemoved.OnNext(entityId);
            }
            
        }

        public void OnEntityAddedToGroup(int entityId)
        {
            if(!IsEntityValid(entityId)) { return; }
            lock (_lock) { CachedEntityIds.Add(entityId); }
            _onEntityAdded.OnNext(entityId);
        }

        public bool Contains(int id)
        {
            lock(_lock)
            { return CachedEntityIds.Contains(id); }
        }

        public IEntity Get(int id)
        {
            lock (_lock)
            { return CachedEntityIds.TryGetValue(id, out var entityId) ? DataSource.Get(entityId) : null; }
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
        { return CachedEntityIds.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}