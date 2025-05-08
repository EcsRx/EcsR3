using System;
using System.Collections;
using R3;
using SystemsR3.Pools;

namespace EcsR3.Components
{
    public class ComponentPool<T> : IComponentPool<T>
        where T : IComponent
    {
        public bool IsStructType { get; }
        
        public PoolConfig PoolConfig { get; }
        public IndexPool IndexPool { get; }
        public T[] Components { get; private set; }
        
        public int Count { get; private set; }
        public int IndexesRemaining => IndexPool.AvailableIndexes.Count;
        
        public Observable<bool> OnPoolExtending => _onPoolExtending;
        private readonly Subject<bool> _onPoolExtending;
        private readonly object _lock = new object();

        public ComponentPool(PoolConfig poolConfig = null)
        {
            PoolConfig = poolConfig ?? new PoolConfig();
            Count = PoolConfig.InitialSize;
            IndexPool = new IndexPool(PoolConfig);
            Components = new T[PoolConfig.InitialSize];
            _onPoolExtending = new Subject<bool>();
            IsStructType = typeof(T).IsValueType;
        }

        public int Allocate()
        {
            lock (_lock)
            {
                if(IndexesRemaining == 0) 
                { Expand(); }
                return IndexPool.AllocateInstance();
            }
        }

        public void Release(int index)
        {
            lock (_lock)
            {
                var instance = Components[index];
            
                if(!IsStructType)
                { Components[index] = default; }
            
                if(instance is IDisposable disposable)
                { disposable.Dispose(); }
            
                IndexPool.ReleaseInstance(index);
            }
        }

        public void Clear()
        {
            IndexPool.Clear();
            Count = PoolConfig.InitialSize;

            for (var i = 0; i < Components.Length; i++)
            {
                if(Components[i] is IDisposable disposable)
                { disposable.Dispose(); }
            }
            
            Components = new T[PoolConfig.InitialSize];
        }

        public void Set(int index, object value)
        {
            lock (_lock)
            { Components.SetValue(value, index); }
        }
        
        public void Expand(int? amountToAdd = null)
        {
            var actualExpansionAmount = amountToAdd ?? PoolConfig.ExpansionSize;
            lock (_lock)
            {
                var newCount = Components.Length + actualExpansionAmount;
                var newEntries = new T[newCount];
                Components.CopyTo(newEntries, 0);
                IndexPool.Expand(newCount-1);
                Components = newEntries;
                Count = newCount;
            }
            
            _onPoolExtending.OnNext(true);
        }

        public IEnumerator GetEnumerator() => Components.GetEnumerator();

        public void Dispose()
        { _onPoolExtending?.Dispose(); }
    }
}