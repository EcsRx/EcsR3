using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using SystemsR3.Pools.Config;

namespace SystemsR3.Pools
{
    public class IdPool : IIdPool
    {
        public PoolConfig PoolConfig { get; }
        
        public int Size => _lastMax;
        
        private int _lastMax;
        private readonly object _lock = new object();
        
        public readonly List<int> AvailableIds;
        
        public Observable<int> OnSizeChanged => _onSizeChanged;
        private Subject<int> _onSizeChanged;

        public IdPool(PoolConfig poolConfig = null)
        {
            PoolConfig = poolConfig ?? new PoolConfig(1000, 3000);
            _lastMax = PoolConfig.InitialSize;
            AvailableIds = Enumerable.Range(1, _lastMax).ToList();
            _onSizeChanged = new Subject<int>();
        }
        
        public int Allocate()
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
        {
            lock(_lock)
            { return id > _lastMax || AvailableIds.Contains(id); }
        }

        public void AllocateSpecificId(int id)
        {
            lock (_lock)
            {
                if(id > _lastMax)
                { Expand(id); }

                AvailableIds.Remove(id);
            }
        }

        public void Release(int id)
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
            lock (_lock)
            {
                var increaseBy = newId -_lastMax ?? PoolConfig.ExpansionSize;
                AvailableIds.AddRange(Enumerable.Range(_lastMax + 1, increaseBy));
                _lastMax += increaseBy + 1;
                _onSizeChanged.OnNext(Size);
            }
        }
        
        public void Clear()
        {
            lock (_lock)
            {
                _lastMax = 0;
                AvailableIds.Clear();
            }
        }

        public void Dispose()
        { _onSizeChanged?.Dispose(); }

        public int[] AllocateMany(int count)
        {
            lock (_lock)
            {
                if(AvailableIds.Count < count)
                { Expand(_lastMax + count); }

                var ids = AvailableIds.Take(count).ToArray();
                AvailableIds.RemoveRange(0, count);
                
                return ids;
            }
        }

        public void ReleaseMany(int[] instances)
        {
            var maxId = 0;
            for (var i = 0; i < instances.Length; i++)
            {
                var id = instances[i];
                
                if(id <= 0)
                { throw new ArgumentException("id has to be >= 1"); }
                
                if(id > maxId){ maxId = id; }
            }
            
            if (maxId > _lastMax)
            { Expand(maxId); }

            lock (_lock)
            { AvailableIds.AddRange(instances); }
        }
    }
}