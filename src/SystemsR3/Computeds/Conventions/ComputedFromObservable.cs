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
            var hasChanged = false;
            lock (Lock)
            { hasChanged = UpdateComputedData(data); }
            
            if(hasChanged)
            { OnDataChanged.OnNext(ComputedData); }
        }
        
        /// <summary>
        /// The method to update the ComputedData from the DataSource
        /// </summary>
        /// <returns>True if the data has changed, false if it has not</returns>
        protected abstract bool UpdateComputedData(TInput data);
    }
}