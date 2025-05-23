using R3;
using SystemsR3.Computeds;
using SystemsR3.Extensions;

namespace EcsR3.Computeds.Entities.Conventions
{
    public abstract class ComputedFromEntityGroup<T> : IComputed<T>
    {
        public T ComputedData;
        public readonly CompositeDisposable Subscriptions;
        
        private readonly Subject<T> _onDataChanged;
        private readonly object _lock = new object();
        
        public IComputedEntityGroup DataSource { get; }
        
        public T Value => ComputedData;
        public Observable<T> OnChanged => _onDataChanged;

        protected ComputedFromEntityGroup(IComputedEntityGroup dataSource)
        {
            DataSource = dataSource;
            Subscriptions = new CompositeDisposable();
            _onDataChanged = new Subject<T>();

            ListenForChanges();
            RefreshData();
        }

        public void ListenForChanges()
        { DataSource.OnChanged.Subscribe(_ => RefreshData()).AddTo(Subscriptions); }

        public void RefreshData()
        {
            lock (_lock)
            {
                var prevousHash = ComputedData.GetHashCode();
                UpdateComputedData();
                var newHash = ComputedData.GetHashCode();
                if (prevousHash == newHash) { return; }
            }

            _onDataChanged.OnNext(ComputedData);
        }
        
        /// <summary>
        /// The method to update the internal computed data
        /// </summary>
        /// <param name="computedEntityGroup">The entity group to process</param>
        /// <returns>The transformed data</returns>
        public abstract void UpdateComputedData();

        public virtual void Dispose()
        {
            Subscriptions.DisposeAll();
            _onDataChanged.Dispose();
        }
    }
}