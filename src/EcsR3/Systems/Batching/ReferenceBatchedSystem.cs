using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Groups;
using EcsR3.Systems.Batching.Accessor;
using SystemsR3.Threading;

namespace EcsR3.Systems.Batching
{
    public abstract class ReferenceBatchedSystem<T1, T2> : ManualBatchedSystem
        where T1 : class, IComponent
        where T2 : class, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2));

        private readonly BatchPoolAccessor<T1, T2> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2> _computedComponentGroup;

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2>(componentDatabase); }

        protected abstract void Process(int entityId, T1 component1, T2 component2);

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2>();
            return _computedComponentGroup;
        }

        protected override void ProcessBatch()
        {
            var (components1, components2) = _batchPoolAccessor.GetPoolArrays();
            var batches = _computedComponentGroup.Value;
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, batches.Length, i =>
                {
                    var batch = batches[i];
                    Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation]);
                });
                return;
            }

            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation]);
            }
        }
    }
    
    public abstract class ReferenceBatchedSystem<T1, T2, T3> : ManualBatchedSystem
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3));
        
        private readonly BatchPoolAccessor<T1, T2, T3> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2, T3> _computedComponentGroup;

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3>(componentDatabase); }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3);

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2, T3>();
            return _computedComponentGroup;
        }

        protected override void ProcessBatch()
        {
            var (components1, components2, components3) = _batchPoolAccessor.GetPoolArrays();
            var batches = _computedComponentGroup.Value;
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, batches.Length, i =>
                {
                    var batch = batches[i];
                    Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], components3[batch.Component3Allocation]);
                });
                return;
            }

            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], components3[batch.Component3Allocation]);
            }
        }
    }
    
    public abstract class ReferenceBatchedSystem<T1, T2, T3, T4> : ManualBatchedSystem
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        
        private readonly BatchPoolAccessor<T1, T2, T3, T4> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2, T3, T4> _computedComponentGroup;

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3, T4>(componentDatabase); }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4);

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4>();
            return _computedComponentGroup;
        }

        protected override void ProcessBatch()
        {
            var (components1, components2, components3, components4) = _batchPoolAccessor.GetPoolArrays();
            var batches = _computedComponentGroup.Value;
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, batches.Length, i =>
                {
                    var batch = batches[i];
                    Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                        components3[batch.Component3Allocation], components4[batch.Component4Allocation]);;
                });
                return;
            }

            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                    components3[batch.Component3Allocation], components4[batch.Component4Allocation]);;
            }
        }
    }
    
    public abstract class ReferenceBatchedSystem<T1, T2, T3, T4, T5> : ManualBatchedSystem
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        
        private readonly BatchPoolAccessor<T1, T2, T3, T4, T5> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2, T3, T4, T5> _computedComponentGroup;

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3, T4, T5>(componentDatabase); }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5);

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4, T5>();
            return _computedComponentGroup;
        }

        protected override void ProcessBatch()
        {
            var (components1, components2, components3, components4, components5) = _batchPoolAccessor.GetPoolArrays();
            var batches = _computedComponentGroup.Value;
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, batches.Length, i =>
                {
                    var batch = batches[i];
                    Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                        components3[batch.Component3Allocation], components4[batch.Component4Allocation],
                        components5[batch.Component5Allocation]);
                });
                return;
            }

            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                    components3[batch.Component3Allocation], components4[batch.Component4Allocation],
                    components5[batch.Component5Allocation]);
            }
        }
    }
    
    public abstract class ReferenceBatchedSystem<T1, T2, T3, T4, T5, T6> : ManualBatchedSystem
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
        where T6 : class, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        
        private readonly BatchPoolAccessor<T1, T2, T3, T4, T5, T6> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2, T3, T4, T5, T6> _computedComponentGroup;

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3, T4, T5, T6>(componentDatabase); }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6);

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4, T5, T6>();
            return _computedComponentGroup;
        }

        protected override void ProcessBatch()
        {
            var (components1, components2, components3, components4, components5,components6) = _batchPoolAccessor.GetPoolArrays();
            var batches = _computedComponentGroup.Value;
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, batches.Length, i =>
                {
                    var batch = batches[i];
                    Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                        components3[batch.Component3Allocation], components4[batch.Component4Allocation],
                        components5[batch.Component5Allocation], components6[batch.Component6Allocation]);
                });
                return;
            }

            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                    components3[batch.Component3Allocation], components4[batch.Component4Allocation],
                    components5[batch.Component5Allocation], components6[batch.Component6Allocation]);
            }
        }
    }
}