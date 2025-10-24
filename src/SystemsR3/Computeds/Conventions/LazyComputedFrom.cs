using R3;
using SystemsR3.Extensions;

namespace SystemsR3.Computeds.Conventions
{
    public abstract class LazyComputedFrom<TOutput, TInput> : ILazyComputed<TOutput>
    {
        public TOutput ComputedData;
        public TInput DataSource { get; }
        
        /// <summary>
        /// Lazily evaluates the data forcing a refresh if it's dirty
        /// </summary>
        public TOutput Value
        {
            get
            {
                if (IsDirty) { ForceUpdate(); }
                return ComputedData;
            }
        }

        public Observable<TOutput> OnChanged => OnDataChanged;
        public Observable<Unit> OnHasChange => OnDataHasChanged;

        protected readonly CompositeDisposable Subscriptions;
        protected readonly Subject<TOutput> OnDataChanged;
        protected readonly Subject<Unit> OnDataHasChanged;
        protected bool IsDirty;
        protected readonly object Lock = new object();
        
        public LazyComputedFrom(TInput dataSource)
        {
            DataSource = dataSource;
            Subscriptions = new CompositeDisposable();
            OnDataChanged = new Subject<TOutput>();
            OnDataHasChanged = new Subject<Unit>();
        }

        public void ForceUpdate()
        {
            bool hasChanged;
            lock (Lock)
            {
                hasChanged = UpdateComputedData();
                IsDirty = false;
            }
            
            if(hasChanged)
            { OnDataChanged.OnNext(ComputedData); }
        }

        /// <summary>
        /// The method to update the ComputedData from the DataSource
        /// </summary>
        /// <returns>True if the data has changed, false if it has not</returns>
        protected abstract bool UpdateComputedData();

        public virtual void Dispose()
        {
            Subscriptions.DisposeAll();
            OnDataChanged.Dispose();
        }
    }
}