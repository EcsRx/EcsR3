namespace SystemsR3.Events.Messages
{
    public interface IMessagePublisher
    {
        /// <summary>
        /// Send Message to all receiver.
        /// </summary>
        void Publish<T>(T message);
    }
}