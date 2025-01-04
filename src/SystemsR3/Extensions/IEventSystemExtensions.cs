using System.Threading.Tasks;
using SystemsR3.Events;
using SystemsR3.Events.Process;

namespace SystemsR3.Extensions
{
    public static class IEventSystemExtensions
    {
        public static Task<T> StartProcess<T>(this IEventSystem eventSystem, IAsyncProcess<T> process)
        { return process.Execute(eventSystem); }
    }
}