using System;
using System.Collections.Generic;
using EcsR3.Groups.Observable;
using R3;
using SystemsR3.Computeds;
using SystemsR3.Extensions;

namespace EcsR3.Computeds
{
    public abstract class ComputedFromGroup<T> : IComputed<T>, IDisposable
    {
        public T CachedData;
        public readonly List<IDisposable> Subscriptions;
        
        private readonly Subject<T> _onDataChanged;
        private readonly object _lock = new object();
        
        public IObservableGroup InternalObservableGroup { get; }

        protected ComputedFromGroup(IObservableGroup internalObservableGroup)
        {
            InternalObservableGroup = internalObservableGroup;
            Subscriptions = new List<IDisposable>();
            
            _onDataChanged = new Subject<T>();

            MonitorChanges();
            RefreshData();
        }
                
        public IDisposable Subscribe(Observer<T> observer)
        { return _onDataChanged.Subscribe(observer); }

        public T Value => GetData();

        public void MonitorChanges()
        {
            InternalObservableGroup.OnEntityAdded.Subscribe(_ => RefreshData()).AddTo(Subscriptions);
            InternalObservableGroup.OnEntityRemoving.Subscribe(_ => RefreshData()).AddTo(Subscriptions);
            RefreshWhen().Subscribe(_ => RefreshData()).AddTo(Subscriptions);
        }

        public void RefreshData()
        {
            lock (_lock)
            {
                var newData = Transform(InternalObservableGroup);
                if (newData.Equals(CachedData)) { return; }
                CachedData = newData;
            }

            _onDataChanged.OnNext(CachedData);
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
        /// The method to generate given data from the data source
        /// </summary>
        /// <param name="observableGroup">The group to process</param>
        /// <returns>The transformed data</returns>
        public abstract T Transform(IObservableGroup observableGroup);

        public T GetData()
        { return CachedData; }

        public virtual void Dispose()
        {
            Subscriptions.DisposeAll();
            _onDataChanged.Dispose();
        }
    }
}