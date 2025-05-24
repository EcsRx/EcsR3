using System;
using R3;
using SystemsR3.Extensions;

namespace SystemsR3.Computeds.Conventions
{
    public abstract class ComputedFromData<TOutput,TInput> : ComputedFrom<TOutput, TInput>
    {
        public ComputedFromData(TInput dataSource) : base(dataSource)
        { Initialize(); }

        public void Initialize()
        {
            RefreshWhen().Subscribe(_ => RefreshData()).AddTo(Subscriptions);
            Observable.Interval(TimeSpan.FromMilliseconds(1)).SubscribeOnce(x => RefreshData());
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
        /// The method to indicate when the listings should be updated
        /// </summary>
        /// <remarks>
        /// If there is no checking required outside of adding/removing this can
        /// return an empty observable, but common usages would be to refresh every update.
        /// The bool is throw away, but is a workaround for not having a Unit class
        /// </remarks>
        /// <returns>An observable trigger that should trigger when the group should refresh</returns>
        protected abstract Observable<Unit> RefreshWhen();
        
        /// <summary>
        /// The method to update the ComputedData from the DataSource
        /// </summary>
        /// <returns>The transformed data</returns>
        protected abstract void UpdateComputedData();
    }
}