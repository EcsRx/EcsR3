using R3;

namespace SystemsR3.Computeds
{
    public interface IComputed<T>
    {        
        T Value { get; }
        Observable<T> OnChanged { get; }
    }
}