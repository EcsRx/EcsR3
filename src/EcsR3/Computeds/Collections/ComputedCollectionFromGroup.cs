using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections.Events;
using EcsR3.Entities;
using EcsR3.Groups.Observable;
using R3;
using SystemsR3.Computeds.Collections;
using SystemsR3.Extensions;

namespace EcsR3.Computeds.Collections
{
    public abstract class ComputedCollectionFromGroup<T> : IComputedCollection<T>, IDisposable
    {
        public IDictionary<int, T> FilteredCache { get; }
        public List<IDisposable> Subscriptions { get; }
        
        public Observable<CollectionElementChangedEvent<T>> OnAdded => _onElementAdded;
        public Observable<CollectionElementChangedEvent<T>> OnRemoved => _onElementChanged;
        public Observable<CollectionElementChangedEvent<T>> OnUpdated => _onElementChanged;
        
        public Observable<IEnumerable<T>> OnChanged => _onDataChanged;

        public IObservableGroup InternalObservableGroup { get; }
        public IEnumerable<T> Value => GetData();

        public T this[int index]
        {
            get
            {
                lock (_lock)
                { return FilteredCache[index]; }
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                { return FilteredCache.Count; }
            }
        }

        private readonly Subject<IEnumerable<T>> _onDataChanged;
        private readonly Subject<CollectionElementChangedEvent<T>> _onElementAdded;
        private readonly Subject<CollectionElementChangedEvent<T>> _onElementChanged;
        private readonly Subject<CollectionElementChangedEvent<T>> _onElementRemoved;
        
        private readonly object _lock = new object();

        protected ComputedCollectionFromGroup(IObservableGroup internalObservableGroup)
        {
            InternalObservableGroup = internalObservableGroup;
            Subscriptions = new List<IDisposable>();       
            FilteredCache = new Dictionary<int, T>();
            
            _onDataChanged = new Subject<IEnumerable<T>>();
            _onElementAdded = new Subject<CollectionElementChangedEvent<T>>();
            _onElementChanged = new Subject<CollectionElementChangedEvent<T>>();
            _onElementRemoved = new Subject<CollectionElementChangedEvent<T>>();
            
            MonitorChanges();
            RefreshData();
        }
        
        
        public IDisposable Subscribe(Observer<IEnumerable<T>> observer)
        { return _onDataChanged.Subscribe(observer); }
        
        public void MonitorChanges()
        {
            InternalObservableGroup.OnEntityAdded.Subscribe(_ => RefreshData()).AddTo(Subscriptions);
            InternalObservableGroup.OnEntityRemoving.Subscribe(_ => RefreshData()).AddTo(Subscriptions);
            RefreshWhen().Subscribe(_ => RefreshData()).AddTo(Subscriptions);
        }
        
        private void ProcessEntity(IEntity entity)
        {
            var isApplicable = ShouldTransform(entity);

            if (!isApplicable)
            {
                lock (_lock)
                {
                    if (!FilteredCache.ContainsKey(entity.Id)) 
                    { return; }
                }

                RemoveEntity(entity.Id);
                return;
            }

            lock (_lock)
            {
                var transformedData = Transform(entity);
                if (FilteredCache.ContainsKey(entity.Id))
                {
                    ChangeEntity(entity.Id, transformedData);
                    return;
                }
    
                AddEntity(entity.Id, transformedData);
            }
        }

        private void AddEntity(int entityId, T transformedData)
        {
            lock (_lock)
            { FilteredCache.Add(entityId, transformedData); }
            
            _onElementAdded.OnNext(new CollectionElementChangedEvent<T>(entityId, default(T), transformedData));
        }

        private void RemoveEntity(int entityId)
        {
            T currentValue;
            lock (_lock)
            {
               currentValue = FilteredCache[entityId];
               FilteredCache.Remove(entityId); 
            }
            
            _onElementRemoved.OnNext(new CollectionElementChangedEvent<T>(entityId, currentValue, default(T)));
        }

        private void ChangeEntity(int entityId, T transformedData)
        {
            T currentData;
            lock (_lock)
            {
                currentData = FilteredCache[entityId];
                FilteredCache[entityId] = transformedData;
            }
            
            _onElementChanged.OnNext(new CollectionElementChangedEvent<T>(entityId, currentData, transformedData));
        }
                       
        public void RefreshData()
        {
            List<int> unprocessedIds;
            lock (_lock)
            {
                unprocessedIds = FilteredCache.Keys.ToList();
                foreach (var entity in InternalObservableGroup)
                {
                    unprocessedIds.Remove(entity.Id);
                    ProcessEntity(entity);
                }
            }

            foreach(var id in unprocessedIds)
            { RemoveEntity(id);}
            
            _onDataChanged.OnNext(FilteredCache.Values);
        }

        /// <summary>
        /// The method to indicate when the listings should be updated
        /// </summary>
        /// <remarks>
        /// If there is no checking required outside of adding/removing this can
        /// return an empty observable, but common usages would be to refresh every update.
        /// The bool is throw away, but is a workaround for not having a Unit class
        /// </remarks>
        /// <returns>An observable trigger that should trigger when the group should refresh</returns>
        public abstract Observable<bool> RefreshWhen();     
        
        /// <summary>
        /// The method to see if this entity should be transformed
        /// </summary>
        /// <param name="entity">The entity to verify</param>
        /// <returns>true if it should transform the entity, false if not</returns>
        public abstract bool ShouldTransform(IEntity entity);
        
        /// <summary>
        /// The method to generate given data from the data source
        /// </summary>
        /// <param name="entity">The entity to transform</param>
        /// <returns>The transformed data</returns>
        public abstract T Transform(IEntity entity);
        
        /// <summary>
        /// Available as a way to post process the data, i.e order them
        /// </summary>
        /// <param name="data">Data to transform</param>
        /// <returns>Processed data</returns>
        public virtual IEnumerable<T> PostProcess(IEnumerable<T> data)
        { return data; }

        public IEnumerable<T> GetData()
        {
            lock (_lock)
            { return PostProcess(FilteredCache.Values); }
        }

        public IEnumerator<T> GetEnumerator()
        { return GetData().GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public void Dispose()
        {
            lock (_lock)
            {
                Subscriptions.DisposeAll();
                _onDataChanged?.Dispose();
                _onElementAdded?.Dispose();
                _onElementChanged?.Dispose();
                _onElementRemoved?.Dispose();
            }
        }
    }
}