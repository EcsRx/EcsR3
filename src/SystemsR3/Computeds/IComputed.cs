using System;
using R3;

namespace SystemsR3.Computeds
{
    public interface IComputed<T>
    {        
        T Value { get; }
        IDisposable Subscribe(Observer<T> observer);
    }
}