using System;
using System.Collections;
using System.Collections.Generic;

namespace SystemsR3.Pools
{
    public abstract class ObjectPool<T> : IObjectPool<T>
        where T : class
    {
        public IndexPool IndexPool { get; }
        public T[] Objects { get; private set; }
        
        public int IndexesRemaining => IndexPool.AvailableIndexes.Count;
        public int ExpansionSize { get; private set; }
        public int MaxSize { get; set; } = int.MaxValue;
        
        public int PopulatedCount { get; private set; }
        
        private readonly object _lock = new object();

        public ObjectPool(int expansionSize) : this(expansionSize, expansionSize)
        { }
        
        public ObjectPool(int expansionSize, int initialSize)
        {
            ExpansionSize = expansionSize;
            IndexPool = new IndexPool(expansionSize, initialSize);
            Objects = new T[initialSize];
        }

        public void PreAllocate(int? allocationAmount = null)
        {
            FillIndexes();
            if(allocationAmount == null) { return; }
            if(allocationAmount <= Objects.Length) { return; }
            
            var clampedAllocationAmount = allocationAmount > MaxSize 
                ? MaxSize : allocationAmount.Value;
            
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

        public void Expand()
        { Expand(ExpansionSize); }
        
        public void Expand(int amountToAdd)
        {
            lock (_lock)
            {
                if(Objects.Length >= MaxSize) { return; }

                var originalCount = Objects.Length;
                var newCount = Objects.Length + amountToAdd;
                if(newCount > MaxSize) { newCount = MaxSize; }
                
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