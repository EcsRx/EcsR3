using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Entities;
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

        public IReadOnlyCollection<IEntity> Value
        {
            get
            {
                lock (_lock)
                { return EnumerableEntities.ToArray(); }
            }
        }
        
        public IEnumerable<IEntity> EnumerableEntities
        {
            get
            {
                lock(_lock)
                { return CachedEntityIds.Select(DataSource.Get); }
            }
        }

        public Observable<IReadOnlyCollection<IEntity>> OnChanged => Observable.Merge(OnAdded, OnRemoved).Select(x => Value);
        
        public Observable<IEntity> OnAdded => _onEntityAdded;
        public Observable<IEntity> OnRemoved => _onEntityRemoved;
        public Observable<IEntity> OnRemoving => _onEntityRemoving;
        
        private readonly Subject<IEntity> _onEntityAdded, _onEntityRemoved, _onEntityRemoving;
        private readonly object _lock = new object();
        
        public ComputedEntityGroupFromEntityGroup(IComputedEntityGroup dataSource)
        {
            Group = dataSource.Group;
            DataSource = dataSource;
            
            _onEntityAdded = new Subject<IEntity>();
            _onEntityRemoved = new Subject<IEntity>();
            _onEntityRemoving = new Subject<IEntity>();

            Subscriptions = new CompositeDisposable();
            CachedEntityIds = new HashSet<int>(dataSource.Where(IsEntityValid).Select(x => x.Id));

            ListenForChanges();
        }

        protected void ListenForChanges()
        {
            DataSource.OnAdded.Subscribe(OnEntityAddedToGroup).AddTo(Subscriptions);
            DataSource.OnRemoving.Subscribe(_onEntityRemoving.OnNext).AddTo(Subscriptions);
            DataSource.OnRemoved.Subscribe(_onEntityRemoved.OnNext).AddTo(Subscriptions);
            
            RefreshWhen().Subscribe(_ => Refresh()).AddTo(Subscriptions);
        }

        public abstract bool IsEntityValid(IEntity entity);
        public virtual Observable<Unit> RefreshWhen() => Observable.Never<Unit>();
        
        public virtual void Refresh()
        {
            foreach (var entity in DataSource)
            {
                var isValid = IsEntityValid(entity);
                var containsEntity = Contains(entity.Id);

                if (isValid)
                {
                    if(containsEntity) { continue; }

                    lock (_lock)
                    { CachedEntityIds.Add(entity.Id); }

                    _onEntityAdded.OnNext(entity);
                    continue;
                }

                if (!containsEntity) { continue; }
                
                lock (_lock)
                { CachedEntityIds.Remove(entity.Id); }
                
                _onEntityRemoving.OnNext(entity);
                _onEntityRemoved.OnNext(entity);
            }
            
        }

        public void OnEntityAddedToGroup(IEntity entity)
        {
            if(!IsEntityValid(entity)) { return; }
            lock (_lock) { CachedEntityIds.Add(entity.Id); }
            _onEntityAdded.OnNext(entity);
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

        public IEnumerator<IEntity> GetEnumerator()
        { return EnumerableEntities.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}