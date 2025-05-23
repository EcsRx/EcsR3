using System;
using System.Collections;
using System.Collections.Generic;
using R3;
using SystemsR3.Extensions;

namespace SystemsR3.Computeds.Conventions.Collections
{
    public abstract class ComputedListFromData<TOutput, TInput> : ComputedFromData<IReadOnlyList<TOutput>, TInput>, IComputedList<TOutput>
    {
        public TOutput this[int index] => ComputedData[index];
        
        public Observable<TOutput> OnAdded { get; }
        public Observable<TOutput> OnRemoved { get; }
        
        public int Count => ComputedData.Count;
        public Observable<IEnumerable<TOutput>> OnChanged => onDataChanged;
        
        private readonly object _lock = new object();
        
        protected readonly Subject<IEnumerable<TOutput>> onDataChanged;

        public ComputedListFromData(TInput dataSource) : base(dataSource)
        {
            DataSource = dataSource;
            Subscriptions = new List<IDisposable>();       
            ComputedData = new List<TOutput>();
            
            onDataChanged = new Subject<IEnumerable<TOutput>>();
            
            ListenForChanges();
            RefreshData();
        }
        
        public void ListenForChanges()
        { RefreshWhen().Subscribe(_ => RefreshData()).AddTo(Subscriptions); }

        public void RefreshData()
        {
            lock (_lock)
            {
                var previousHash = ComputedData.GetHashCode();
                UpdateComputedData();
                var newHash = ComputedData.GetHashCode();
                if(newHash == previousHash) { return; }
            }

            onDataChanged.OnNext(ComputedData);
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
        public abstract Observable<Unit> RefreshWhen();     
        
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
        public abstract void UpdateComputedData();

        public IEnumerator<TOutput> GetEnumerator()
        { return Value.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public void Dispose()
        {
            Subscriptions.DisposeAll();
            onDataChanged?.Dispose();
        }
    }
}