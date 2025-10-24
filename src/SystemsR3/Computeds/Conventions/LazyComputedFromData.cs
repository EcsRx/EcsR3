using R3;

namespace SystemsR3.Computeds.Conventions
{
    public abstract class LazyComputedFromData<TOutput,TInput> : LazyComputedFrom<TOutput, TInput>
    {
        public LazyComputedFromData(TInput dataSource) : base(dataSource)
        { Initialize(); }

        public void Initialize()
        { RefreshWhen().Subscribe(_ => { lock (Lock) { IsDirty = true; } }).AddTo(Subscriptions); }
        
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
    }
}