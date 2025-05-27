using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using SystemsR3.Pools.Config;

namespace SystemsR3.Pools
{
    public class IndexPool : IIndexPool
    {
        public PoolConfig PoolConfig { get; }
        
        public int Size => _lastMax;
        
        private int _lastMax;
        private readonly object _lock = new object();
        
        public Observable<int> OnSizeChanged => _onSizeChanged;
        private Subject<int> _onSizeChanged;
        
        public readonly Stack<int> AvailableIndexes;

        public IndexPool(PoolConfig poolConfig = null)
        {
            PoolConfig = poolConfig ?? new PoolConfig(1000, 100);
            _lastMax = PoolConfig.InitialSize;
            AvailableIndexes = new Stack<int>(Enumerable.Range(0, _lastMax).Reverse());
            _onSizeChanged = new Subject<int>();
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
        
        public IReadOnlyList<int> AllocateMany(int count)
        {
            lock (_lock)
            {
                if(AvailableIndexes.Count < count)
                { Expand(_lastMax + count); }

                return AvailableIndexes.Take(count).ToArray();
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
        
        public void ReleaseMany(IReadOnlyList<int> instances)
        {
            var maxId = 0;
            for (var i = 0; i < instances.Count; i++)
            {
                var id = instances[i];
                
                if(id <= 0)
                { throw new ArgumentException("id has to be >= 1"); }
                
                if(id > maxId){ maxId = id; }
            }
            
            if (maxId > _lastMax)
            { Expand(maxId); }

            lock (_lock)
            {
                for (var i = 0; i < instances.Count; i++)
                { AvailableIndexes.Push(instances[i]); }
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
            _onSizeChanged.OnNext(Size);
        }

        public void Clear()
        {
            lock (_lock)
            {
                _lastMax = 0;
                AvailableIndexes.Clear();
            }
        }
        
        public void Dispose()
        { _onSizeChanged?.Dispose(); }
    }
}