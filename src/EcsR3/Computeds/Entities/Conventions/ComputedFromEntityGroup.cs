using R3;
using SystemsR3.Computeds;
using SystemsR3.Extensions;

namespace EcsR3.Computeds.Entities
{
    public abstract class ComputedFromEntityGroup<T> : IComputed<T>
    {
        public T CachedData;
        public readonly CompositeDisposable Subscriptions;
        
        private readonly Subject<T> _onDataChanged;
        private readonly object _lock = new object();
        
        public IComputedEntityGroup InternalComputedGroup { get; }
        public Observable<T> OnChanged => _onDataChanged;

        protected ComputedFromEntityGroup(IComputedEntityGroup internalComputedGroup)
        {
            InternalComputedGroup = internalComputedGroup;
            Subscriptions = new CompositeDisposable();
            _onDataChanged = new Subject<T>();

            MonitorChanges();
            RefreshData();
        }

        public T Value => GetData();

        public void MonitorChanges()
        {
            InternalComputedGroup.OnAdded.Subscribe(_ => RefreshData()).AddTo(Subscriptions);
            InternalComputedGroup.OnRemoved.Subscribe(_ => RefreshData()).AddTo(Subscriptions);
            RefreshWhen().Subscribe(_ => RefreshData()).AddTo(Subscriptions);
        }

        public void RefreshData()
        {
            lock (_lock)
            {
                var newData = Transform(InternalComputedGroup);
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
        /// <param name="computedEntityGroup">The entity group to process</param>
        /// <returns>The transformed data</returns>
        public abstract T Transform(IComputedEntityGroup computedEntityGroup);

        public T GetData()
        { return CachedData; }

        public virtual void Dispose()
        {
            Subscriptions.DisposeAll();
            _onDataChanged.Dispose();
        }
    }
}