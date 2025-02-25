﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SystemsR3.Pools
{   
    public class IndexPool : IPool<int>
    {
        public int IncrementSize => _increaseSize;
        
        private int _lastMax;
        private readonly int _increaseSize;
        private readonly object _lock = new object();
        
        public readonly Stack<int> AvailableIndexes;

        public IndexPool(int increaseSize = 100, int startingSize = 1000)
        {
            _lastMax = startingSize;
            _increaseSize = increaseSize;
            AvailableIndexes = new Stack<int>(Enumerable.Range(0, _lastMax).Reverse());
        }
        
        public int AllocateInstance()
        {
            lock (_lock)
            {
                if(AvailableIndexes.Count == 0)
                { Expand(); }
            
                return AvailableIndexes.Pop();
            }
        }

        public void ReleaseInstance(int index)
        {
            if(index < 0)
            { throw new ArgumentException("id has to be >= 0"); }

            lock (_lock)
            {
                if (index > _lastMax)
                { Expand(index); }
            
                AvailableIndexes.Push(index);
            }
        }

        public void Expand(int? newIndex = null)
        {
            var increaseBy = (newIndex+1) -_lastMax ?? _increaseSize;
            if (increaseBy <= 0){ return; }
            
            var newEntries = Enumerable.Range(_lastMax, increaseBy).Reverse();
            
            foreach(var entry in newEntries)
            { AvailableIndexes.Push(entry); }
            
            _lastMax += increaseBy;
        }

        public void Clear()
        {
            lock (_lock)
            {
                _lastMax = 0;
                AvailableIndexes.Clear();
            }
        }
    }
}