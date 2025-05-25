using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Groups;
using EcsR3.Systems.Batching.Accessor;
using SystemsR3.Threading;

namespace EcsR3.Systems.Batching
{
        public abstract class RawBatchedSystem<T1, T2> : ManualBatchedSystem
        where T1 : IComponent
        where T2 : IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2));

        private readonly BatchPoolAccessor<T1, T2> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2> _computedComponentGroup;

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2>(componentDatabase); }

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2>();
            return _computedComponentGroup;
        }

        protected override void ProcessBatch()
        {
            var componentArrays = _batchPoolAccessor.GetPoolArrays();
            ProcessGroup(_computedComponentGroup.Value, componentArrays);
        }
        
        protected abstract void ProcessGroup(ComponentBatch<T1, T2>[] componentBatches, (T1[], T2[]) componentPools);
    }
    
    public abstract class RawBatchedSystem<T1, T2, T3> : ManualBatchedSystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3));
        
        private readonly BatchPoolAccessor<T1, T2, T3> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2, T3> _computedComponentGroup;

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3>(componentDatabase); }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3);

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
        
        protected abstract void ProcessGroup(ComponentBatch<T1, T2, T3>[] componentBatches, (T1[], T2[], T3[]) componentPools);
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

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3, T4>(componentDatabase); }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4);

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
        
        protected abstract void ProcessGroup(ComponentBatch<T1, T2, T3, T4>[] componentBatches, (T1[], T2[], T3[], T4[]) componentPools);
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

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3, T4, T5>(componentDatabase); }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5);

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
        
        protected abstract void ProcessGroup(ComponentBatch<T1, T2, T3, T4, T5>[] componentBatches, (T1[], T2[], T3[], T4[], T5[]) componentPools);
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

        protected RawBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3, T4, T5, T6>(componentDatabase); }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6);

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
        
        protected abstract void ProcessGroup(ComponentBatch<T1, T2, T3, T4, T5, T6>[] componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[]) componentPools);
    }
}