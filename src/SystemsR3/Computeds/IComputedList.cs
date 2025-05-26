using System.Collections.Generic;
using R3;

namespace SystemsR3.Computeds
{
    /// <summary>
    /// Represents a computed list of elements
    /// </summary>
    /// <typeparam name="T">The data to contain</typeparam>
    public interface IComputedList<T> : IComputed<IReadOnlyList<T>>, IReadOnlyList<T>
    {
        /// <summary>
        /// Event stream for when an element has been added to this collection
        /// </summary>
        Observable<T> OnAdded { get; }
        
        /// <summary>
        /// Event stream for when an element has been removed from this collection
        /// </summary>
        Observable<T> OnRemoved { get; }
    }
}