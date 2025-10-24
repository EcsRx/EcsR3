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
        
        public readonly HashSet<Entity> CachedEntityIds;
        public readonly CompositeDisposable Subscriptions;
        public readonly IComputedEntityGroup DataSource;
        public readonly IEntityComponentAccessor EntityComponentAccessor;

        public IReadOnlyCollection<Entity> Value
        {
            get
            {
                lock (_lock)
                { return CachedEntityIds; }
            }
        }

        public Observable<IReadOnlyCollection<Entity>> OnChanged => Observable.Merge(OnAdded, OnRemoved).Select(x => Value);
        public Observable<Unit> OnHasChanged => Observable.Merge(OnAdded, OnRemoved).Select(x => Unit.Default);
        
        public Observable<Entity> OnAdded => _onEntityAdded;
        public Observable<Entity> OnRemoved => _onEntityRemoved;
        public Observable<Entity> OnRemoving => _onEntityRemoving;

        private readonly Subject<Entity> _onEntityAdded, _onEntityRemoved, _onEntityRemoving;
        private readonly object _lock = new object();
        
        public ComputedEntityGroupFromEntityGroup(IEntityComponentAccessor entityComponentAccessor, IComputedEntityGroup dataSource)
        {
            Group = dataSource.Group;
            DataSource = dataSource;
            EntityComponentAccessor = entityComponentAccessor;

            _onEntityAdded = new Subject<Entity>();
            _onEntityRemoved = new Subject<Entity>();
            _onEntityRemoving = new Subject<Entity>();

            Subscriptions = new CompositeDisposable();
            CachedEntityIds = new HashSet<Entity>(dataSource.Where(IsEntityValid));

            ListenForChanges();
        }

        protected void ListenForChanges()
        {
            DataSource.OnAdded.Subscribe(OnEntityAddedToGroup).AddTo(Subscriptions);
            DataSource.OnRemoving.Subscribe(_onEntityRemoving.OnNext).AddTo(Subscriptions);
            DataSource.OnRemoved.Subscribe(_onEntityRemoved.OnNext).AddTo(Subscriptions);
            
            RefreshWhen().Subscribe(_ => Refresh()).AddTo(Subscriptions);
        }

        public abstract bool IsEntityValid(Entity entity);
        public virtual Observable<Unit> RefreshWhen() => Observable.Never<Unit>();
        
        public virtual void Refresh()
        {
            foreach (var entity in DataSource)
            {
                var isValid = IsEntityValid(entity);
                var containsEntity = Contains(entity);

                if (isValid)
                {
                    if(containsEntity) { continue; }

                    lock (_lock)
                    { CachedEntityIds.Add(entity); }

                    _onEntityAdded.OnNext(entity);
                    continue;
                }

                if (!containsEntity) { continue; }
                
                lock (_lock)
                { CachedEntityIds.Remove(entity); }
                
                _onEntityRemoving.OnNext(entity);
                _onEntityRemoved.OnNext(entity);
            }
            
        }

        public void OnEntityAddedToGroup(Entity entity)
        {
            if(!IsEntityValid(entity)) { return; }
            lock (_lock) { CachedEntityIds.Add(entity); }
            _onEntityAdded.OnNext(entity);
        }

        public bool Contains(Entity entity)
        {
            lock(_lock)
            { return CachedEntityIds.Contains(entity); }
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

        public IEnumerator<Entity> GetEnumerator()
        { return CachedEntityIds.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}