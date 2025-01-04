using System;

namespace SystemsR3.Computeds
{
    public interface IComputed<out T>
    {        
        T Value { get; }
    }
}