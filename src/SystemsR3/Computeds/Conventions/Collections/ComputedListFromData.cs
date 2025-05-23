using System;
using System.Collections;
using System.Collections.Generic;
using R3;
using SystemsR3.Extensions;

namespace SystemsR3.Computeds.Conventions.Collections
{
    public abstract class ComputedListFromData<TOutput, TInput> : ComputedFromData<IReadOnlyList<TOutput>, TInput>, IComputedList<TOutput>
    {
        public TOutput this[int index] => ComputedData[index];
        
        public Observable<TOutput> OnAdded { get; }
        public Observable<TOutput> OnRemoved { get; }
        
        public int Count => ComputedData.Count;
        
        public ComputedListFromData(TInput dataSource) : base(dataSource)
        {}

        public IEnumerator<TOutput> GetEnumerator()
        { return Value.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}