using System;
using R3;

namespace SystemsR3.Extensions
{
    public static class ObservableExtensions
    {
        public static void SubscribeOnce<T>(this Observable<T> observable, Action<T> action)
        { observable.Take(1).Subscribe(action); }
    }
}