using R3;

namespace SystemsR3.Events.Messages
{
    public interface IMessageReceiver
    {
        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        Observable<T> Receive<T>();
    }
}