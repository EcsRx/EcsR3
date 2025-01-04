using System;
using System.Collections.Generic;

namespace SystemsR3.Extensions
{
    public static class IDictionaryExtensions
    {       
        public static void RemoveAndDispose<T>(this IDictionary<T, IDisposable> disposables, T key)
        {
            var disposable = disposables[key];
            disposables.Remove(key);
            disposable?.Dispose();
        }
        
        public static void RemoveAndDisposeAll<T>(this IDictionary<T, IDisposable> disposables)
        {
            disposables.ForEachRun(x => x.Value?.Dispose());
            disposables.Clear();
        }
    }
}