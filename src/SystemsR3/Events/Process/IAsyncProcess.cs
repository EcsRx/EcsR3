using System.Threading.Tasks;

namespace SystemsR3.Events.Process
{
    /// <summary>
    /// The IAsyncProcess represents a long term request/response style process using events.
    /// </summary>
    /// <remarks>
    /// This is mainly used for when you want to send an event but require a response/notification of the event being handled and completed.
    /// For example you may need to tell a view component to move, which may take a few seconds to happen, so with this interface
    /// you can start the process, then wait for the response before doing something else.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncProcess<T>
    {
        Task<T> Execute(IEventSystem eventSystem);
    }
}