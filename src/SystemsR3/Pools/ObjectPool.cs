using System;
using System.Collections;
using System.Collections.Generic;

namespace SystemsR3.Pools
{
    public abstract class ObjectPool<T> : IObjectPool<T>
        where T : class
    {
        public PoolConfig PoolConfig { get; }
        public IndexPool IndexPool { get; }
        public T[] Objects { get; private set; }
        
        public int IndexesRemaining => IndexPool.AvailableIndexes.Count;
        public int PopulatedCount { get; private set; }
        
        private readonly object _lock = new object();

        public ObjectPool(PoolConfig poolConfig = null)
        {
            PoolConfig = poolConfig ?? new PoolConfig(100, 100);
            IndexPool = new IndexPool(poolConfig);
            Objects = new T[PoolConfig.InitialSize];
        }

        public void PreAllocate(int? allocationAmount = null)
        {
            FillIndexes();
            if(allocationAmount == null) { return; }
            if(allocationAmount <= Objects.Length) { return; }
            
            var clampedAllocationAmount = allocationAmount > PoolConfig.MaxSize
                ? PoolConfig.MaxSize : allocationAmount.Value;
            
            var actualAllocation = clampedAllocationAmount - Objects.Length;
            if(actualAllocation <= 0) { return; }
            
            Expand(actualAllocation);
        }

        public T Allocate()
        {
            lock (_lock)
            {
                if(PopulatedCount < Objects.Length)
                { FillIndexes(PopulatedCount); }
                
                if(IndexesRemaining == 0) 
                { Expand(); }
                
                if(IndexesRemaining == 0)
                { return null; }
                
                var index = IndexPool.AllocateInstance();
                var instance = Objects[index];
                OnAllocated(instance);
                return instance;
            }
        }
        
        public abstract T Create();
        public abstract void Destroy(T instance);
        
        public virtual void OnAllocated(T instance) {}
        public virtual void OnReleased(T instance) {}

        public void Release(T instance)
        {
            lock (_lock)
            {
                var indexOfInstance = Array.IndexOf(Objects, instance);
                OnReleased(instance);
                IndexPool.ReleaseInstance(indexOfInstance);
            }
        }

        public void Clear()
        {
            IndexPool.Clear();

            for (var i = 0; i < Objects.Length; i++)
            { Destroy(Objects[i]); }
            
            Objects = Array.Empty<T>();
        }

        public void FillIndexes(int startIndex = 0)
        {
            lock (_lock)
            {
                for (var i = startIndex; i < Objects.Length; i++)
                {
                    var newInstance = Create();
                    Objects[i] = newInstance;
                }
                PopulatedCount = Objects.Length;
            }
        }
        
        public void Expand(int? amountToAdd = null)
        {
            lock (_lock)
            {
                if(Objects.Length >= PoolConfig.MaxSize) { return; }

                var originalCount = Objects.Length;
                var newCount = Objects.Length + (amountToAdd ?? PoolConfig.ExpansionSize);
                if(newCount > PoolConfig.MaxSize) { newCount = PoolConfig.MaxSize; }
                
                var newEntries = new T[newCount];            
                Objects.CopyTo(newEntries, 0);
                IndexPool.Expand(newCount-1);
                Objects = newEntries;
                FillIndexes(originalCount);
            }
        }

        public IEnumerator<T> GetEnumerator()
        { return ((IEnumerable<T>)Objects).GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}