using R3;
using SystemsR3.Events.Messages;
using SystemsR3.Threading;

namespace SystemsR3.Events
{
    public class EventSystem : IEventSystem
    {
        public IMessageBroker MessageBroker { get; }
        public IThreadHandler ThreadHandler { get; }

        public EventSystem(IMessageBroker messageBroker, IThreadHandler threadHandler)
        {
            MessageBroker = messageBroker;
            ThreadHandler = threadHandler;
        }

        public void Publish<T>(T message)
        { MessageBroker.Publish(message); }

        public void PublishAsync<T>(T message)
        { ThreadHandler.Run(() => MessageBroker.Publish(message)); }

        public Observable<T> Receive<T>()
        { return MessageBroker.Receive<T>(); }

    }
}