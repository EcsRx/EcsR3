using System;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Systems.Batching.Accessor;
using SystemsR3.Threading;

namespace EcsR3.Systems.Batching
{
    public abstract class RawBatchedSystem<T1> : ManualBatchedSystem
        where T1 : IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1));

        protected readonly BatchPoolAccessor<T1> BatchPoolAccessor;
        protected IComputedComponentGroup<T1> CastComponentGroup => ComputedComponentGroup as IComputedComponentGroup<T1>;

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        { BatchPoolAccessor = new BatchPoolAccessor<T1>(componentDatabase); }

        protected override IComputedComponentGroup GetComponentGroup()
        { return ComputedComponentGroupRegistry.GetComputedGroup<T1>(); }

        protected override void ProcessBatch()
        {
            var componentArrays = BatchPoolAccessor.GetPoolArrays();
            ProcessGroup(CastComponentGroup.Value, componentArrays);
        }
        
        protected abstract void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1>> componentBatches, T1[] componentPools);
    }
    
    public abstract class RawBatchedSystem<T1, T2> : ManualBatchedSystem
        where T1 : IComponent
        where T2 : IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2));

        protected readonly BatchPoolAccessor<T1, T2> BatchPoolAccessor;
        protected IComputedComponentGroup<T1, T2> CastComponentGroup => ComputedComponentGroup as IComputedComponentGroup<T1, T2>;

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        { BatchPoolAccessor = new BatchPoolAccessor<T1, T2>(componentDatabase); }

        protected override IComputedComponentGroup GetComponentGroup()
        { return ComputedComponentGroupRegistry.GetComputedGroup<T1, T2>(); }

        protected override void ProcessBatch()
        {
            var componentArrays = BatchPoolAccessor.GetPoolArrays();
            ProcessGroup(CastComponentGroup.Value, componentArrays);
        }
        
        protected abstract void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2>> componentBatches, (T1[], T2[]) componentPools);
    }
    
    public abstract class RawBatchedSystem<T1, T2, T3> : ManualBatchedSystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3));
        
        private readonly BatchPoolAccessor<T1, T2, T3> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2, T3> _computedComponentGroup;

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3>(componentDatabase); }

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2, T3>();
            return _computedComponentGroup;
        }
        
        protected override void ProcessBatch()
        {
            var componentArrays = _batchPoolAccessor.GetPoolArrays();
            ProcessGroup(_computedComponentGroup.Value, componentArrays);
        }
        
        protected abstract void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3>> componentBatches, (T1[], T2[], T3[]) componentPools);
    }
    
    public abstract class RawBatchedSystem<T1, T2, T3, T4> : ManualBatchedSystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        
        private readonly BatchPoolAccessor<T1, T2, T3, T4> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2, T3, T4> _computedComponentGroup;

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3, T4>(componentDatabase); }

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4>();
            return _computedComponentGroup;
        }
        
        protected override void ProcessBatch()
        {
            var componentArrays = _batchPoolAccessor.GetPoolArrays();
            ProcessGroup(_computedComponentGroup.Value, componentArrays);
        }
        
        protected abstract void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4>> componentBatches, (T1[], T2[], T3[], T4[]) componentPools);
    }
    
    public abstract class RawBatchedSystem<T1, T2, T3, T4, T5> : ManualBatchedSystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        
        private readonly BatchPoolAccessor<T1, T2, T3, T4, T5> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2, T3, T4, T5> _computedComponentGroup;

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3, T4, T5>(componentDatabase); }

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4, T5>();
            return _computedComponentGroup;
        }
        
        protected override void ProcessBatch()
        {
            var componentArrays = _batchPoolAccessor.GetPoolArrays();
            ProcessGroup(_computedComponentGroup.Value, componentArrays);
        }
        
        protected abstract void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5>> componentBatches, (T1[], T2[], T3[], T4[], T5[]) componentPools);
    }
    
    public abstract class RawBatchedSystem<T1, T2, T3, T4, T5, T6> : ManualBatchedSystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        
        private readonly BatchPoolAccessor<T1, T2, T3, T4, T5, T6> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2, T3, T4, T5, T6> _computedComponentGroup;

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3, T4, T5, T6>(componentDatabase); }

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4, T5, T6>();
            return _computedComponentGroup;
        }

        protected override void ProcessBatch()
        {
            var componentArrays = _batchPoolAccessor.GetPoolArrays();
            ProcessGroup(_computedComponentGroup.Value, componentArrays);
        }
        
        protected abstract void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[]) componentPools);
    }
    
    public abstract class RawBatchedSystem<T1, T2, T3, T4, T5, T6, T7> : ManualBatchedSystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
        where T7 : IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        
        private readonly BatchPoolAccessor<T1, T2, T3, T4, T5, T6, T7> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7> _computedComponentGroup;

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3, T4, T5, T6, T7>(componentDatabase); }

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4, T5, T6, T7>();
            return _computedComponentGroup;
        }

        protected override void ProcessBatch()
        {
            var componentArrays = _batchPoolAccessor.GetPoolArrays();
            ProcessGroup(_computedComponentGroup.Value, componentArrays);
        }
        
        protected abstract void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6, T7>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[], T7[]) componentPools);
    }
}