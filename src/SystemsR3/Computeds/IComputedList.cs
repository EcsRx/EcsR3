using R3;
using SystemsR3.Computeds.Collections.Events;

namespace SystemsR3.Computeds.Collections
{
    /// <summary>
    /// Represents a computed list of elements
    /// </summary>
    /// <typeparam name="T">The data to contain</typeparam>
    public interface IComputedList<T> : IComputedCollection<T>
    {
        /// <summary>
        /// Event stream for when an element has been changes in this list
        /// </summary>
        Observable<CollectionElementChangedEvent<T>> OnValueChanged { get; }
        
        /// <summary>
        /// Access an element via its index
        /// </summary>
        /// <param name="index"></param>
        T this[int index] { get; }
    }
}