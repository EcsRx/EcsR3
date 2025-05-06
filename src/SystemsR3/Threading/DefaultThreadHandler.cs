using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SystemsR3.Threading
{
    public class DefaultThreadHandler : IThreadHandler
    {
        public void For(int start, int end, Action<int> process) => Parallel.For(start, end, process);
        public Task Run(Action process) => Task.Run(process);
        public void ForEach<T>(IEnumerable<T> data, Action<T> action) => Parallel.ForEach(data, action);
    }
}