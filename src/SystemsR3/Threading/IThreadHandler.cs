using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SystemsR3.Threading
{
    public interface IThreadHandler
    {
        void For(int start, int end, Action<int> process);
        void ForEach<T>(IEnumerable<T> data, Action<T> action);
        Task Run(Action process);
    }
}