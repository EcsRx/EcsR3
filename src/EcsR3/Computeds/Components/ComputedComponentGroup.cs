using System;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Conventions;
using EcsR3.Extensions;
using EcsR3.Groups;

namespace EcsR3.Computeds.Components
{
    public class ComputedComponentGroup<T1> : ComputedDataFromEntityGroup<ComponentBatch<T1>[]>, IComputedComponentGroup<T1>
    {
        private readonly int _t1ComponentId;
        
        public IComponentDatabase ComponentDatabase { get; }
        
        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IComputedEntityGroup computedEntityGroup) : base(computedEntityGroup)
        {
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            
            if(!DataSource.Group.Matches(_t1ComponentId)) 
            { throw new ArgumentException("ComputedEntityGroup must match component types"); }
            
            ComponentDatabase = componentDatabase;
            Group = DataSource.Group;
        }

        protected override void UpdateComputedData()
        {
            ComputedData = new ComponentBatch<T1>[DataSource.Value.Count];
            var index = 0;
            foreach (var entity in DataSource)
            { ComputedData[index++] = new ComponentBatch<T1>(entity.Id, entity.ComponentAllocations[_t1ComponentId]); }
        }
    }

    public class ComputedComponentGroup<T1, T2> : ComputedDataFromEntityGroup<ComponentBatch<T1, T2>[]>,
        IComputedComponentGroup<T1, T2>
    {
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;

        public IComponentDatabase ComponentDatabase { get; }

        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IComputedEntityGroup computedEntityGroup) : base(computedEntityGroup)
        {
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            _t2ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T2));

            if (!DataSource.Group.Matches(_t1ComponentId, _t2ComponentId))
            {
                throw new ArgumentException("ComputedEntityGroup must match component types");
            }

            ComponentDatabase = componentDatabase;
            Group = DataSource.Group;
        }

        protected override void UpdateComputedData()
        {
            ComputedData = new ComponentBatch<T1, T2>[DataSource.Value.Count];
            var index = 0;
            foreach (var entity in DataSource)
            {
                ComputedData[index++] = new ComponentBatch<T1, T2>(entity.Id,
                    entity.ComponentAllocations[_t1ComponentId],
                    entity.ComponentAllocations[_t2ComponentId]);
            }
        }
    }

    public class ComputedComponentGroup<T1, T2, T3> : ComputedDataFromEntityGroup<ComponentBatch<T1, T2, T3>[]>,
        IComputedComponentGroup<T1, T2, T3>
    {
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;

        public IComponentDatabase ComponentDatabase { get; }

        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentDatabase componentDatabase,
            IComponentTypeLookup componentTypeLookup, IComputedEntityGroup computedEntityGroup) : base(
            computedEntityGroup)
        {
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            _t2ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T2));
            _t3ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T3));

            if (!DataSource.Group.Matches(_t1ComponentId, _t2ComponentId, _t3ComponentId))
            {
                throw new ArgumentException("ComputedEntityGroup must match component types");
            }

            ComponentDatabase = componentDatabase;
            Group = DataSource.Group;
        }

        protected override void UpdateComputedData()
        {
            ComputedData = new ComponentBatch<T1, T2, T3>[DataSource.Value.Count];
            var index = 0;
            foreach (var entity in DataSource)
            {
                ComputedData[index++] = new ComponentBatch<T1, T2, T3>(entity.Id,
                    entity.ComponentAllocations[_t1ComponentId],
                    entity.ComponentAllocations[_t2ComponentId],
                    entity.ComponentAllocations[_t3ComponentId]);
            }
        }
    }
    
    public class ComputedComponentGroup<T1, T2, T3, T4> : ComputedDataFromEntityGroup<ComponentBatch<T1, T2, T3, T4>[]>,
        IComputedComponentGroup<T1, T2, T3, T4>
    {
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;
        private readonly int _t4ComponentId;

        public IComponentDatabase ComponentDatabase { get; }

        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentDatabase componentDatabase,
            IComponentTypeLookup componentTypeLookup, IComputedEntityGroup computedEntityGroup) : base(
            computedEntityGroup)
        {
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            _t2ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T2));
            _t3ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T3));
            _t4ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T4));

            if (!DataSource.Group.Matches(_t1ComponentId, _t2ComponentId, _t3ComponentId, _t4ComponentId))
            {
                throw new ArgumentException("ComputedEntityGroup must match component types");
            }

            ComponentDatabase = componentDatabase;
            Group = DataSource.Group;
        }

        protected override void UpdateComputedData()
        {
            ComputedData = new ComponentBatch<T1, T2, T3, T4>[DataSource.Value.Count];
            var index = 0;
            foreach (var entity in DataSource)
            {
                ComputedData[index++] = new ComponentBatch<T1, T2, T3, T4>(entity.Id,
                    entity.ComponentAllocations[_t1ComponentId],
                    entity.ComponentAllocations[_t2ComponentId],
                    entity.ComponentAllocations[_t3ComponentId],
                    entity.ComponentAllocations[_t4ComponentId]);
            }
        }
    }
}