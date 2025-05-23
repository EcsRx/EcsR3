using System;
using System.Collections.Generic;
using R3;
using SystemsR3.Extensions;

namespace SystemsR3.Computeds.Data
{
    public abstract class ComputedFromData<TOutput,TInput> : IComputed<TOutput>, IDisposable
    {
        public TOutput CachedData;
        protected readonly List<IDisposable> Subscriptions;
        
        private readonly Subject<TOutput> _onDataChanged;
        private readonly object _lock = new object();
        
        public TInput DataSource { get; }
        public Observable<TOutput> OnChanged => _onDataChanged;
        
        public ComputedFromData(TInput dataSource)
        {
            DataSource = dataSource;
            Subscriptions = new List<IDisposable>();
            _onDataChanged = new Subject<TOutput>();

            MonitorChanges();
            RefreshData();
        }
        
        public TOutput Value => GetData();

        public void MonitorChanges()
        {
            RefreshWhen().Subscribe(_ => RefreshData()).AddTo(Subscriptions);
        }

        public void RefreshData()
        {
            lock (_lock)
            {
                var newData = Transform(DataSource);
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
        public abstract Observable<Unit> RefreshWhen();
        
        /// <summary>
        /// The method to generate given data from the data source
        /// </summary>
        /// <param name="dataSource">The source of data to work off</param>
        /// <returns>The transformed data</returns>
        public abstract TOutput Transform(TInput dataSource);

        public TOutput GetData()
        { return CachedData; }

        public virtual void Dispose()
        {
            Subscriptions.DisposeAll();
            _onDataChanged.Dispose();
        }
    }
}