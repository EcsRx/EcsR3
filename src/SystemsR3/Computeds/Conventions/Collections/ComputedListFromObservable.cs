using System.Collections;
using System.Collections.Generic;
using R3;

namespace SystemsR3.Computeds.Conventions.Collections
{
    public abstract class ComputedListFromObservable<TOutput, TInput> : ComputedFromObservable<IReadOnlyList<TOutput>, TInput>, IComputedList<TOutput>
    {
        public TOutput this[int index] => ComputedData[index];
        
        public Observable<TOutput> OnAdded { get; protected set; }
        public Observable<TOutput> OnRemoved { get; protected set; }
        
        public int Count => ComputedData.Count;
        
        public ComputedListFromObservable(Observable<TInput> dataSource) : base(dataSource)
        {}

        public IEnumerator<TOutput> GetEnumerator()
        { return Value.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}