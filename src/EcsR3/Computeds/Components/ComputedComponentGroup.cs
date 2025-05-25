using System;
using EcsR3.Components;
using EcsR3.Components.Lookups;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Conventions;
using EcsR3.Extensions;
using EcsR3.Groups;
using R3;

namespace EcsR3.Computeds.Components
{
    public class ComputedComponentGroup<T1> : ComputedFromEntityGroup<ComponentBatch<T1>[]>, IComputedComponentGroup<T1> where T1 : IComponent
    {
        private readonly int _t1ComponentId;
        
        public LookupGroup Group { get; }
        
        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IComputedEntityGroup computedEntityGroup) : base(computedEntityGroup)
        {
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            
            if(!DataSource.Group.Matches(_t1ComponentId))
            { throw new ArgumentException("ComputedEntityGroup must match component types"); }
            
            Group = DataSource.Group;
            RefreshData();
        }

        protected override void UpdateComputedData()
        {
            ComputedData = new ComponentBatch<T1>[DataSource.Value.Count];
            var index = 0;
            foreach (var entity in DataSource)
            { ComputedData[index++] = new ComponentBatch<T1>(entity.Id, entity.ComponentAllocations[_t1ComponentId]); }
        }
    }

    public class ComputedComponentGroup<T1, T2> : ComputedFromEntityGroup<ComponentBatch<T1, T2>[]>,
        IComputedComponentGroup<T1, T2> 
        where T1 : IComponent
        where T2 : IComponent
    {
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;

        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup,
            IComputedEntityGroup computedEntityGroup) : base(computedEntityGroup)
        {
            _t1ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T1));
            _t2ComponentId = componentTypeLookup.GetComponentTypeId(typeof(T2));

            if (!DataSource.Group.Matches(_t1ComponentId, _t2ComponentId))
            {
                throw new ArgumentException("ComputedEntityGroup must match component types");
            }

            Group = DataSource.Group;
            RefreshData();
        }

        protected override void UpdateComputedData()
        {
            ComputedData = new ComponentBatch<T1, T2>[DataSource.Count];
            var index = 0;
            foreach (var entity in DataSource)
            {
                ComputedData[index++] = new ComponentBatch<T1, T2>(entity.Id,
                    entity.ComponentAllocations[_t1ComponentId],
                    entity.ComponentAllocations[_t2ComponentId]);
            }
        }
    }

    public class ComputedComponentGroup<T1, T2, T3> : ComputedFromEntityGroup<ComponentBatch<T1, T2, T3>[]>,
        IComputedComponentGroup<T1, T2, T3>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;

        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IComputedEntityGroup computedEntityGroup) : base(
            computedEntityGroup)
        {
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
    
    public class ComputedComponentGroup<T1, T2, T3, T4> : ComputedFromEntityGroup<ComponentBatch<T1, T2, T3, T4>[]>,
        IComputedComponentGroup<T1, T2, T3, T4>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
    {
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;
        private readonly int _t4ComponentId;

        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IComputedEntityGroup computedEntityGroup) : base(
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

            Group = DataSource.Group;
            RefreshData();
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
    
    public class ComputedComponentGroup<T1, T2, T3, T4, T5> : ComputedFromEntityGroup<ComponentBatch<T1, T2, T3, T4, T5>[]>,
        IComputedComponentGroup<T1, T2, T3, T4, T5>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
    {
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;
        private readonly int _t4ComponentId;
        private readonly int _t5ComponentId;

        public LookupGroup Group { get; }

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IComputedEntityGroup computedEntityGroup) : base(
            computedEntityGroup)
        {
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
            ComputedData = new ComponentBatch<T1, T2, T3, T4, T5>[DataSource.Value.Count];
            var index = 0;
            foreach (var entity in DataSource)
            {
                ComputedData[index++] = new ComponentBatch<T1, T2, T3, T4, T5>(entity.Id,
                    entity.ComponentAllocations[_t1ComponentId],
                    entity.ComponentAllocations[_t2ComponentId],
                    entity.ComponentAllocations[_t3ComponentId],
                    entity.ComponentAllocations[_t4ComponentId],
                    entity.ComponentAllocations[_t5ComponentId]);
            }
        }
    }
    
    public class ComputedComponentGroup<T1, T2, T3, T4, T5, T6> : ComputedFromEntityGroup<ComponentBatch<T1, T2, T3, T4, T5, T6>[]>,
        IComputedComponentGroup<T1, T2, T3, T4, T5, T6>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
    {
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;
        private readonly int _t4ComponentId;
        private readonly int _t5ComponentId;
        private readonly int _t6ComponentId;

        public LookupGroup Group { get; }
        public Observable<Unit> OnRefreshed => OnChanged.Select(x => Unit.Default);

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IComputedEntityGroup computedEntityGroup) : base(
            computedEntityGroup)
        {
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
            ComputedData = new ComponentBatch<T1, T2, T3, T4, T5, T6>[DataSource.Value.Count];
            var index = 0;
            foreach (var entity in DataSource)
            {
                ComputedData[index++] = new ComponentBatch<T1, T2, T3, T4, T5, T6>(entity.Id,
                    entity.ComponentAllocations[_t1ComponentId],
                    entity.ComponentAllocations[_t2ComponentId],
                    entity.ComponentAllocations[_t3ComponentId],
                    entity.ComponentAllocations[_t4ComponentId],
                    entity.ComponentAllocations[_t5ComponentId],
                    entity.ComponentAllocations[_t6ComponentId]);
            }
        }
    }
    
    public class ComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7> : ComputedFromEntityGroup<ComponentBatch<T1, T2, T3, T4, T5, T6, T7>[]>,
        IComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
        where T7 : IComponent
    {
        private readonly int _t1ComponentId;
        private readonly int _t2ComponentId;
        private readonly int _t3ComponentId;
        private readonly int _t4ComponentId;
        private readonly int _t5ComponentId;
        private readonly int _t6ComponentId;
        private readonly int _t7ComponentId;

        public LookupGroup Group { get; }
        public Observable<Unit> OnRefreshed => OnChanged.Select(x => Unit.Default);

        public ComputedComponentGroup(IComponentTypeLookup componentTypeLookup, IComputedEntityGroup computedEntityGroup) : base(
            computedEntityGroup)
        {
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
            ComputedData = new ComponentBatch<T1, T2, T3, T4, T5, T6, T7>[DataSource.Value.Count];
            var index = 0;
            foreach (var entity in DataSource)
            {
                ComputedData[index++] = new ComponentBatch<T1, T2, T3, T4, T5, T6, T7>(entity.Id,
                    entity.ComponentAllocations[_t1ComponentId],
                    entity.ComponentAllocations[_t2ComponentId],
                    entity.ComponentAllocations[_t3ComponentId],
                    entity.ComponentAllocations[_t4ComponentId],
                    entity.ComponentAllocations[_t5ComponentId],
                    entity.ComponentAllocations[_t6ComponentId],
                    entity.ComponentAllocations[_t7ComponentId]);
            }
        }
    }
}