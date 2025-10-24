using R3;

namespace SystemsR3.Computeds
{
    public interface ILazyComputed<T> : IComputed<T>
    {        
        Observable<Unit> OnHasChange { get; }
        void ForceRefresh();
    }
}