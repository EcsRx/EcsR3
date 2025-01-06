using System;
using SystemsR3.Computeds;
using SystemsR3.Observers;

namespace SystemsR3.Extensions
{
    public static class IComputedExtensions
    {
        public static IDisposable Subscribe<T>(this IComputed<T> computed, Action<T> onNext)
        { return computed.Subscribe(new BasicObserver<T>(onNext)); }
        
        public static IDisposable Subscribe<T>(this IComputed<T> computed, Action<T> onNext, Action<Exception> onError)
        { return computed.Subscribe(new BasicObserver<T>(onNext, onError)); }
    }
}