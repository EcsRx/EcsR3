using System.Collections.Generic;
using R3;

namespace SystemsR3.Computeds.Collections
{
    public interface IComputedCollection<T> : IComputed<IEnumerable<T>>, IEnumerable<T>
    {
        /// <summary>
        /// Event stream for when an element has been added to this collection
        /// </summary>
        Observable<T> OnAdded { get; }
        
        /// <summary>
        /// Event stream for when an element has been removed from this collection
        /// </summary>
        Observable<T> OnRemoved { get; }
        
        /// <summary>
        /// How many elements are within the collection
        /// </summary>
        int Count { get; }
    }
}