using System;
using EcsR3.Collections.Entities;
using EcsR3.Components;
using EcsR3.Components.Lookups;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Conventions;
using EcsR3.Extensions;
using EcsR3.Groups;
using R3;

namespace EcsR3.Computeds.Components
{
    public class ComputedComponentGroup<T1> : ComputedFromEntityGroup<ReadOnlyMemory<ComponentBatch<T1>>>, IComputedComponentGroup<T1> where T1 : IComponent
    {
        protected IEntityAllocationDatabase AllocationDatabase { get; }
        
        private readonly int _t1ComponentId;
        
        public LookupGroup Group { get; }
        
        private ComponentBatch<T1>[] _internalCache = Array.Empty<ComponentBatch<T1>>();
        
        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IEntityAllocationDatabase allocationDatabase, IComputedEntityGroup computedEntityGroup) : base(computedEntityGroup)
        {
            AllocationDatabase = allocationDatabase;
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            
            if(!DataSource.Group.Matches(_t1ComponentId))
            { throw new ArgumentException("ComputedEntityGroup must match component types"); }
            
            Group = DataSource.Group;
            RefreshData();
        }

        protected override void UpdateComputedData()
        {
            if(DataSource.Count > _internalCache.Length)
            { Array.Resize(ref _internalCache, DataSource.Count); }
            
            var indexesUsed = 0;
            foreach (var entityId in DataSource)
            { _internalCache[indexesUsed++] = new ComponentBatch<T1>(entityId, AllocationDatabase.GetEntityComponentAllocation(_t1ComponentId, entityId)); }
            
            ComputedData = new ReadOnlyMemory<ComponentBatch<T1>>(_internalCache, 0, indexesUsed);
        }
    }

    public class ComputedComponentGroup<T1, T2> : ComputedFromEntityGroup<ReadOnlyMemory<ComponentBatch<T1, T2>>>,
        IComputedComponentGroup<T1, T2> 
        where T1 : IComponent
        where T2 : IComponent
    {
        protected IEntityAllocationDatabase AllocationDatabase { get; }
        
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private ComponentBatch<T1, T2>[] _internalCache = Array.Empty<ComponentBatch<T1, T2>>();

        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IEntityAllocationDatabase allocationDatabase, 
            IComputedEntityGroup computedEntityGroup) : base(computedEntityGroup)
        {
            AllocationDatabase = allocationDatabase;
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            _t2ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T2));

            if (!DataSource.Group.Matches(_t1ComponentId, _t2ComponentId))
            {
                throw new ArgumentException("ComputedEntityGroup must match component types");
            }

            Group = DataSource.Group;
            ComputedData = new ReadOnlyMemory<ComponentBatch<T1,T2>>(_internalCache);
            RefreshData();
        }

        protected override void UpdateComputedData()
        {
            if(DataSource.Count > _internalCache.Length)
            { Array.Resize(ref _internalCache, DataSource.Count); }
            
            var indexesUsed = 0;
            foreach (var entityId in DataSource)
            {
                _internalCache[indexesUsed++] = new ComponentBatch<T1, T2>(entityId,
                    AllocationDatabase.GetEntityComponentAllocation(_t1ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t2ComponentId, entityId));
            }
            
            ComputedData = new ReadOnlyMemory<ComponentBatch<T1,T2>>(_internalCache, 0, indexesUsed);
        }
    }

    public class ComputedComponentGroup<T1, T2, T3> : ComputedFromEntityGroup<ReadOnlyMemory<ComponentBatch<T1, T2, T3>>>,
        IComputedComponentGroup<T1, T2, T3>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        protected IEntityAllocationDatabase AllocationDatabase { get; }
        
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;
        private ComponentBatch<T1, T2, T3>[] _internalCache = Array.Empty<ComponentBatch<T1, T2, T3>>();

        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IEntityAllocationDatabase allocationDatabase, IComputedEntityGroup computedEntityGroup) : base(
            computedEntityGroup)
        {
            AllocationDatabase = allocationDatabase;
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            _t2ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T2));
            _t3ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T3));

            if (!DataSource.Group.Matches(_t1ComponentId, _t2ComponentId, _t3ComponentId))
            {
                throw new ArgumentException("ComputedEntityGroup must match component types");
            }

            Group = DataSource.Group;
            RefreshData();
        }

        protected override void UpdateComputedData()
        {
            if(DataSource.Count > _internalCache.Length)
            { Array.Resize(ref _internalCache, DataSource.Count); }
            
            var indexesUsed = 0;
            foreach (var entityId in DataSource)
            {
                _internalCache[indexesUsed++] = new ComponentBatch<T1, T2, T3>(entityId,
                    AllocationDatabase.GetEntityComponentAllocation(_t1ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t2ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t3ComponentId, entityId));
            }
            
            ComputedData = new ReadOnlyMemory<ComponentBatch<T1,T2, T3>>(_internalCache, 0, indexesUsed);
        }
    }
    
    public class ComputedComponentGroup<T1, T2, T3, T4> : ComputedFromEntityGroup<ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4>>>,
        IComputedComponentGroup<T1, T2, T3, T4>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
    {
        protected IEntityAllocationDatabase AllocationDatabase { get; }
        
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;
        private readonly int _t4ComponentId;
        private ComponentBatch<T1, T2, T3, T4>[] _internalCache = Array.Empty<ComponentBatch<T1, T2, T3, T4>>();

        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IEntityAllocationDatabase allocationDatabase, IComputedEntityGroup computedEntityGroup) : base(
            computedEntityGroup)
        {
            AllocationDatabase = allocationDatabase;
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            _t2ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T2));
            _t3ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T3));
            _t4ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T4));

            if (!DataSource.Group.Matches(_t1ComponentId, _t2ComponentId, _t3ComponentId, _t4ComponentId))
            {
                throw new ArgumentException("ComputedEntityGroup must match component types");
            }

            Group = DataSource.Group;
            RefreshData();
        }

        protected override void UpdateComputedData()
        {
            if(DataSource.Count > _internalCache.Length)
            { Array.Resize(ref _internalCache, DataSource.Count); }
            
            var indexesUsed = 0;
            foreach (var entityId in DataSource)
            {
                _internalCache[indexesUsed++] = new ComponentBatch<T1, T2, T3, T4>(entityId,
                    AllocationDatabase.GetEntityComponentAllocation(_t1ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t2ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t3ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t4ComponentId, entityId));
            }
            
            ComputedData = new ReadOnlyMemory<ComponentBatch<T1,T2,T3,T4>>(_internalCache, 0, indexesUsed);
        }
    }
    
    public class ComputedComponentGroup<T1, T2, T3, T4, T5> : ComputedFromEntityGroup<ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5>>>,
        IComputedComponentGroup<T1, T2, T3, T4, T5>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
    {
        protected IEntityAllocationDatabase AllocationDatabase { get; }
        
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;
        private readonly int _t4ComponentId;
        private readonly int _t5ComponentId;
        private ComponentBatch<T1, T2, T3, T4, T5>[] _internalCache = Array.Empty<ComponentBatch<T1, T2, T3, T4, T5>>();
        
        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IEntityAllocationDatabase allocationDatabase, IComputedEntityGroup computedEntityGroup) : base(
            computedEntityGroup)
        {
            AllocationDatabase = allocationDatabase;
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            _t2ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T2));
            _t3ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T3));
            _t4ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T4));
            _t5ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T5));

            if (!DataSource.Group.Matches(_t1ComponentId, _t2ComponentId, _t3ComponentId, _t4ComponentId, _t5ComponentId))
            {
                throw new ArgumentException("ComputedEntityGroup must match component types");
            }

            Group = DataSource.Group;
            RefreshData();
        }

        protected override void UpdateComputedData()
        {
            if(DataSource.Count > _internalCache.Length)
            { Array.Resize(ref _internalCache, DataSource.Count); }
            
            var indexesUsed = 0;
            foreach (var entityId in DataSource)
            {
                _internalCache[indexesUsed++] = new ComponentBatch<T1, T2,T3,T4,T5>(entityId,
                    AllocationDatabase.GetEntityComponentAllocation(_t1ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t2ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t3ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t4ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t5ComponentId, entityId));
            }
            
            ComputedData = new ReadOnlyMemory<ComponentBatch<T1,T2,T3,T4,T5>>(_internalCache, 0, indexesUsed);
        }
    }
    
    public class ComputedComponentGroup<T1, T2, T3, T4, T5, T6> : ComputedFromEntityGroup<ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6>>>,
        IComputedComponentGroup<T1, T2, T3, T4, T5, T6>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
    {
        protected IEntityAllocationDatabase AllocationDatabase { get; }
        
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;
        private readonly int _t4ComponentId;
        private readonly int _t5ComponentId;
        private readonly int _t6ComponentId;
        private ComponentBatch<T1, T2, T3, T4,T5,T6>[] _internalCache = Array.Empty<ComponentBatch<T1, T2, T3,T4,T5,T6>>();

        public LookupGroup Group { get; }
        public Observable<Unit> OnRefreshed => OnChanged.Select(x => Unit.Default);

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IEntityAllocationDatabase allocationDatabase, IComputedEntityGroup computedEntityGroup) : base(
            computedEntityGroup)
        {
            AllocationDatabase = allocationDatabase;
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            _t2ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T2));
            _t3ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T3));
            _t4ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T4));
            _t5ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T5));
            _t6ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T6));

            if (!DataSource.Group.Matches(_t1ComponentId, _t2ComponentId, _t3ComponentId, _t4ComponentId, _t5ComponentId, _t6ComponentId))
            {
                throw new ArgumentException("ComputedEntityGroup must match component types");
            }

            Group = DataSource.Group;
            RefreshData();
        }

        protected override void UpdateComputedData()
        {
            if(DataSource.Count > _internalCache.Length)
            { Array.Resize(ref _internalCache, DataSource.Count); }
            
            var indexesUsed = 0;
            foreach (var entityId in DataSource)
            {
                _internalCache[indexesUsed++] = new ComponentBatch<T1, T2,T3,T4,T5,T6>(entityId,
                    AllocationDatabase.GetEntityComponentAllocation(_t1ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t2ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t3ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t4ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t5ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t6ComponentId, entityId));
            }
            
            ComputedData = new ReadOnlyMemory<ComponentBatch<T1,T2,T3,T4,T5,T6>>(_internalCache, 0, indexesUsed);
        }
    }
    
    public class ComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7> : ComputedFromEntityGroup<ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6, T7>>>,
        IComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
        where T7 : IComponent
    {
        protected IEntityAllocationDatabase AllocationDatabase { get; }
        
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;
        private readonly int _t4ComponentId;
        private readonly int _t5ComponentId;
        private readonly int _t6ComponentId;
        private readonly int _t7ComponentId;
        private ComponentBatch<T1, T2, T3, T4,T5,T6,T7>[] _internalCache = Array.Empty<ComponentBatch<T1, T2, T3,T4,T5,T6,T7>>();

        public LookupGroup Group { get; }
        public Observable<Unit> OnRefreshed => OnChanged.Select(x => Unit.Default);

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IEntityAllocationDatabase allocationDatabase, IComputedEntityGroup computedEntityGroup) : base(
            computedEntityGroup)
        {
            AllocationDatabase = allocationDatabase;
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            _t2ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T2));
            _t3ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T3));
            _t4ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T4));
            _t5ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T5));
            _t6ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T6));
            _t7ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T7));

            if (!DataSource.Group.Matches(_t1ComponentId, _t2ComponentId, _t3ComponentId, _t4ComponentId, _t5ComponentId, _t6ComponentId, _t7ComponentId))
            {
                throw new ArgumentException("ComputedEntityGroup must match component types");
            }

            Group = DataSource.Group;
            RefreshData();
        }

        protected override void UpdateComputedData()
        {
            if(DataSource.Count > _internalCache.Length)
            { Array.Resize(ref _internalCache, DataSource.Count); }
            
            var indexesUsed = 0;
            foreach (var entityId in DataSource)
            {
                _internalCache[indexesUsed++] = new ComponentBatch<T1, T2,T3,T4,T5,T6,T7>(entityId,
                    AllocationDatabase.GetEntityComponentAllocation(_t1ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t2ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t3ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t4ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t5ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t6ComponentId, entityId),
                    AllocationDatabase.GetEntityComponentAllocation(_t7ComponentId, entityId));
            }
            
            ComputedData = new ReadOnlyMemory<ComponentBatch<T1,T2,T3,T4,T5,T6,T7>>(_internalCache, 0, indexesUsed);
        }
    }
}