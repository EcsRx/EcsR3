using R3;
using SystemsR3.Extensions;

namespace SystemsR3.Computeds.Conventions
{
    public abstract class ComputedFromData<TOutput,TInput> : IComputed<TOutput>
    {
        public TOutput ComputedData;
        public TInput DataSource { get; }

        public TOutput Value => ComputedData;
        public Observable<TOutput> OnChanged => OnDataChanged;
        
        protected readonly CompositeDisposable Subscriptions;
        protected readonly Subject<TOutput> OnDataChanged;
        protected readonly object Lock = new object();
        
        
        public ComputedFromData(TInput dataSource)
        {
            DataSource = dataSource;
            Subscriptions = new CompositeDisposable();
            OnDataChanged = new Subject<TOutput>();

            Initialize();
        }

        public void Initialize()
        {
            ListenForChanges();
            RefreshData();
        }
        
        public void RefreshData()
        {
            lock (Lock)
            {
                var previousHash = ComputedData.GetHashCode();
                UpdateComputedData();
                var newHash = ComputedData.GetHashCode();
                if (previousHash == newHash) { return; }
            }
            
            OnDataChanged.OnNext(ComputedData);
        }
        

        /// <summary>
        /// Start listening for changes
        /// </summary>
        public abstract void ListenForChanges();
        
        /// <summary>
        /// The method to update the ComputedData from the DataSource
        /// </summary>
        /// <returns>The transformed data</returns>
        public abstract void UpdateComputedData();

        public virtual void Dispose()
        {
            Subscriptions.DisposeAll();
            OnDataChanged.Dispose();
        }
    }
}