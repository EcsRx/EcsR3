using System;
using System.Collections;
using System.Collections.Generic;
using R3;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace EcsR3.Components
{
    public class ComponentPool<T> : IComponentPool<T>
        where T : IComponent
    {
        public bool IsStructType { get; }

        public Type ComponentType { get; } = typeof(T);
        
        public PoolConfig PoolConfig { get; }
        public IndexPool IndexPool { get; }
        public T[] Components => InternalComponents;

        public T[] InternalComponents;
        
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
            InternalComponents = new T[PoolConfig.InitialSize];
            _onPoolExtending = new Subject<bool>();
            IsStructType = typeof(T).IsValueType;
        }

        public int Allocate()
        {
            lock (_lock)
            {
                if(IndexesRemaining == 0) 
                { Expand(); }
                return IndexPool.Allocate();
            }
        }

        public int[] Allocate(int count)
        {
            if(count > IndexesRemaining) 
            { Expand(count); }
            
            lock (_lock)
            { return IndexPool.AllocateMany(count); }
        }

        public void Release(int index)
        {
            lock (_lock)
            {
                var instance = InternalComponents[index];
            
                if(!IsStructType)
                { InternalComponents[index] = default; }
            
                if(instance is IDisposable disposable)
                { disposable.Dispose(); }
            
                IndexPool.Release(index);
            }
        }

        public void Release(int[] indexes)
        {
            lock (_lock)
            {
                for (var i = 0; i < indexes.Length; i++)
                {
                    var index = indexes[i];
                    var instance = InternalComponents[index];
            
                    if(!IsStructType)
                    { InternalComponents[index] = default; }
            
                    if(instance is IDisposable disposable)
                    { disposable.Dispose(); }
                }
                IndexPool.ReleaseMany(indexes);
            }
        }

        public void Clear()
        {
            IndexPool.Clear();
            Count = PoolConfig.InitialSize;

            for (var i = 0; i < InternalComponents.Length; i++)
            {
                if(InternalComponents[i] is IDisposable disposable)
                { disposable.Dispose(); }
            }
            
            InternalComponents = new T[PoolConfig.InitialSize];
        }

        public void Set(int index, IComponent value)
        {
            lock (_lock)
            { InternalComponents.SetValue(value, index); }
        }
        
        public IComponent Get(int index)
        {
            lock (_lock)
            { return InternalComponents[index]; }
        }

        
        public void Set(int index, T value)
        {
            lock(_lock)
            { InternalComponents[index] = value; }
        }

        public void Set(int[] indexes, IReadOnlyList<T> value)
        {
            lock (_lock)
            {
                for (var i = 0; i < indexes.Length; i++)
                {
                    var allocationIndex = indexes[i];
                    InternalComponents[allocationIndex] = value[i];
                }
            }
        }
        
        public void Expand(int? amountToAdd = null)
        {
            var actualExpansionAmount = amountToAdd ?? PoolConfig.ExpansionSize;
            lock (_lock)
            {
                var newCount = InternalComponents.Length + actualExpansionAmount;
                Array.Resize(ref InternalComponents, newCount);
                IndexPool.Expand(newCount-1);
                Count = newCount;
            }
            
            _onPoolExtending.OnNext(true);
        }

        public IEnumerator GetEnumerator() => InternalComponents.GetEnumerator();

        public void Dispose()
        { _onPoolExtending?.Dispose(); }
    }
}