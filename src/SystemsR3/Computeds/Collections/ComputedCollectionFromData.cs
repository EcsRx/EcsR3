using System;
using System.Collections;
using System.Collections.Generic;
using R3;
using SystemsR3.Extensions;

namespace SystemsR3.Computeds.Collections
{
    public abstract class ComputedCollectionFromData<TInput, TOutput> : IComputedCollection<TOutput>, IDisposable
    {
        public IList<TOutput> ComputedData { get; }
        public List<IDisposable> Subscriptions { get; }
        
        public TInput DataSource { get; }
        public IEnumerable<TOutput> Value => GetData();
        
        public TOutput this[int index] => ComputedData[index];
        public int Count => ComputedData.Count;
        public Observable<IEnumerable<TOutput>> OnChanged => onDataChanged;
        
        protected readonly Subject<IEnumerable<TOutput>> onDataChanged;

        public ComputedCollectionFromData(TInput dataSource)
        {
            DataSource = dataSource;
            Subscriptions = new List<IDisposable>();       
            ComputedData = new List<TOutput>();
            
            onDataChanged = new Subject<IEnumerable<TOutput>>();
            
            MonitorChanges();
            RefreshData();
        }
        
        public void MonitorChanges()
        { RefreshWhen().Subscribe(_ => RefreshData()).AddTo(Subscriptions); }

        public void RefreshData()
        { Transform(DataSource); }

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
        /// The method to populate ComputedData and raise events from the data source
        /// </summary>
        /// <remarks>
        /// Unfortunately this is not as clever as the other computed classes
        /// and is unable to really work out whats added/removed etc
        /// so it is up to the consumer to trigger the events and populate
        /// the ComputedData object
        /// </remarks>
        /// <param name="dataSource">The dataSource to transform</param>
        public abstract void Transform(TInput dataSource);

        public IEnumerable<TOutput> GetData()
        { return ComputedData; }

        public IEnumerator<TOutput> GetEnumerator()
        { return GetData().GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public void Dispose()
        {
            Subscriptions.DisposeAll();
            onDataChanged?.Dispose();
        }
    }
}