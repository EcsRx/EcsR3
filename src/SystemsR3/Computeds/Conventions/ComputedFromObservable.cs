using R3;
using SystemsR3.Extensions;

namespace SystemsR3.Computeds.Conventions
{
    public abstract class ComputedFromObservable<TOutput, TInput> : IComputed<TOutput>
    {
        public TOutput ComputedData;
        public Observable<TInput> DataSource { get; }

        public TOutput Value => ComputedData;
        public Observable<TOutput> OnChanged => OnDataChanged;
        
        protected readonly CompositeDisposable Subscriptions;
        protected readonly Subject<TOutput> OnDataChanged;
        protected readonly object Lock = new object();
        
        
        public ComputedFromObservable(Observable<TInput> dataSource)
        {
            DataSource = dataSource;
            Subscriptions = new CompositeDisposable();
            OnDataChanged = new Subject<TOutput>();

            DataSource.Subscribe(RefreshData).AddTo(Subscriptions);
        }
        
        public void RefreshData(TInput data)
        {
            lock (Lock)
            {
                var previousHash = ComputedData.GetHashCode();
                UpdateComputedData(data);
                var newHash = ComputedData.GetHashCode();
                if (previousHash == newHash) { return; }
            }
            
            OnDataChanged.OnNext(ComputedData);
        }
        
        /// <summary>
        /// The method to update the ComputedData from the DataSource
        /// </summary>
        /// <returns>The transformed data</returns>
        public abstract void UpdateComputedData(TInput data);

        public virtual void Dispose()
        {
            Subscriptions.DisposeAll();
            OnDataChanged.Dispose();
        }
    }
}