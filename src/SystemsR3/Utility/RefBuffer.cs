using System;
using System.Collections.Generic;

namespace SystemsR3.Utility
{
    public class RefBuffer<T> where T : struct
    {
        private readonly Memory<T> _data;
        private readonly int[] _indexes;
            
        public int Count => _indexes.Length;

        public RefBuffer(Memory<T> data, int[] indexes)
        {
            _data = data;
            _indexes = indexes;
        }
            
        public ref T this[int index] => ref _data.Span[_indexes[index]];
    }
}