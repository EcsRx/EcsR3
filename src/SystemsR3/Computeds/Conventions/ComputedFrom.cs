using R3;
using SystemsR3.Extensions;

namespace SystemsR3.Computeds.Conventions
{
    public abstract class ComputedFrom<TOutput, TInput> : IComputed<TOutput>
    {
        public TOutput ComputedData;
        public TInput DataSource { get; }

        public TOutput Value => ComputedData;
        public Observable<TOutput> OnChanged => OnDataChanged;
        
        protected readonly CompositeDisposable Subscriptions;
        protected readonly Subject<TOutput> OnDataChanged;
        protected readonly object Lock = new object();
        
        public ComputedFrom(TInput dataSource)
        {
            DataSource = dataSource;
            Subscriptions = new CompositeDisposable();
            OnDataChanged = new Subject<TOutput>();
        }

        public virtual void Dispose()
        {
            Subscriptions.DisposeAll();
            OnDataChanged.Dispose();
        }
    }
}