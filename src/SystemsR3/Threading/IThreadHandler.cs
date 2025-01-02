using System;
using System.Threading.Tasks;

namespace SystemsR3.Threading
{
    public interface IThreadHandler
    {
        void For(int start, int end, Action<int> process);
        Task Run(Action process);
    }
}