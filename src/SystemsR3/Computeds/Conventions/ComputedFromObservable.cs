using R3;
using SystemsR3.Extensions;

namespace SystemsR3.Computeds.Conventions
{
    public abstract class ComputedFromObservable<TOutput, TInput> : ComputedFrom<TOutput, Observable<TInput>>
    {
        public ComputedFromObservable(Observable<TInput> dataSource) : base(dataSource)
        { DataSource.Subscribe(RefreshData).AddTo(Subscriptions); }
        
        protected void RefreshData(TInput data)
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
        protected abstract void UpdateComputedData(TInput data);
    }
}