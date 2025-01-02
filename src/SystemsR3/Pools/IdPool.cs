using System;
using System.Collections.Generic;
using System.Linq;

namespace SystemsR3.Pools
{
    public class IdPool : IIdPool
    {
        public int IncrementSize => _increaseSize;
        
        private int _lastMax;
        private readonly int _increaseSize;
        private readonly object _lock = new object();
        
        public readonly List<int> AvailableIds;

        public IdPool(int increaseSize = 10000, int startingSize = 10000)
        {
            _lastMax = startingSize;
            _increaseSize = increaseSize;
            AvailableIds = Enumerable.Range(1, _lastMax).ToList();
        }

        public int AllocateInstance()
        {
            lock (_lock)
            {
                if(AvailableIds.Count == 0)
                { Expand(); }
            
                var id = AvailableIds[0];
                AvailableIds.RemoveAt(0);

                return id;
            }
        }

        public bool IsAvailable(int id)
        { return id > _lastMax || AvailableIds.Contains(id); }

        public void AllocateSpecificId(int id)
        {
            lock (_lock)
            {
                if(id > _lastMax)
                { Expand(id); }

                AvailableIds.Remove(id);
            }
        }

        public void ReleaseInstance(int id)
        {
            if(id <= 0)
            { throw new ArgumentException("id has to be >= 1"); }

            lock (_lock)
            {
                if (id > _lastMax)
                { Expand(id); }
            
                AvailableIds.Add(id);
            }
        }

        public void Expand(int? newId = null)
        {
            var increaseBy = newId -_lastMax ?? _increaseSize;
            AvailableIds.AddRange(Enumerable.Range(_lastMax + 1, increaseBy));
            _lastMax += increaseBy + 1;
        }
    }
}