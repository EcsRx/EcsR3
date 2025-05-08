using System;
using System.Collections.Generic;
using System.Linq;

namespace SystemsR3.Pools
{   
    public class IndexPool : IPool<int>
    {
        public PoolConfig PoolConfig { get; }
        
        private int _lastMax;
        private readonly object _lock = new object();
        
        public readonly Stack<int> AvailableIndexes;

        public IndexPool(PoolConfig poolConfig = null)
        {
            PoolConfig = poolConfig ?? new PoolConfig(1000, 100);
            _lastMax = PoolConfig.InitialSize;
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
            { throw new ArgumentException("index has to be >= 0"); }

            lock (_lock)
            {
                if (AvailableIndexes.Contains(index))
                { return; }

                if (index > _lastMax)
                { return; }
                
                AvailableIndexes.Push(index);
            }
        }

        public void Expand(int? newIndex = null)
        {
            var increaseBy = (newIndex+1) -_lastMax ?? PoolConfig.ExpansionSize;
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