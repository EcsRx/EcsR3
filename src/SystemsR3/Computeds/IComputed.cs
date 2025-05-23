using System;
using R3;

namespace SystemsR3.Computeds
{
    public interface IComputed<T> : IDisposable
    {        
        T Value { get; }
        Observable<T> OnChanged { get; }
    }
}