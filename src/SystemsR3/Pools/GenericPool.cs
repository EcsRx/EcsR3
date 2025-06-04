using System;
using System.Collections;
using System.Collections.Generic;
using R3;
using SystemsR3.Pools.Config;

namespace SystemsR3.Pools
{
    public abstract class GenericPool<T> : IGenericPool<T>
    {
        public PoolConfig PoolConfig { get; }
        public IndexPool IndexPool { get; }
        public T[] Objects;
        
        public int IndexesRemaining => IndexPool.AvailableIndexes.Count;
        public int PopulatedCount { get; private set; }
        public int Size
        {
            get { lock(_lock){ return Objects.Length; } }
        }
        
        public Observable<int> OnSizeChanged => _onSizeChanged;
        private Subject<int> _onSizeChanged;

        private readonly object _lock = new object();

        public GenericPool(PoolConfig poolConfig = null)
        {
            PoolConfig = poolConfig ?? new PoolConfig(100, 100);
            IndexPool = new IndexPool(PoolConfig);
            Objects = new T[PoolConfig.InitialSize];
            _onSizeChanged = new Subject<int>();
        }

        public void PreAllocate(int? allocationAmount = null)
        {
            FillIndexes();
            int length;
            lock (_lock) { length = Objects.Length; }
            
            if(allocationAmount == null) { return; }
            if(allocationAmount <= length) { return; }
            
            var clampedAllocationAmount = allocationAmount > PoolConfig.MaxSize
                ? PoolConfig.MaxSize : allocationAmount.Value;
            
            var actualAllocation = clampedAllocationAmount - length;
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
                { return default; }
                
                var index = IndexPool.Allocate();
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
                IndexPool.Release(indexOfInstance);
            }
        }

        public void Clear()
        {
            IndexPool.Clear();

            lock (_lock)
            {
                for (var i = 0; i < Objects.Length; i++)
                { Destroy(Objects[i]); }
            
                Objects = Array.Empty<T>();
            }
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
                
                Array.Resize(ref Objects, newCount);
                IndexPool.Expand(newCount-1);
                FillIndexes(originalCount);
                _onSizeChanged.OnNext(Size);
            }
        }

        public IEnumerator<T> GetEnumerator()
        { return ((IEnumerable<T>)Objects).GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
        
        public void Dispose()
        { _onSizeChanged?.Dispose(); }
    }
}