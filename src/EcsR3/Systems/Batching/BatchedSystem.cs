using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Groups;
using EcsR3.Systems.Batching.Accessor;
using SystemsR3.Threading;

namespace EcsR3.Systems.Batching
{
    public abstract class BatchedSystem<T1, T2> : ManualBatchedSystem
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2));
        
        private readonly BatchPoolAccessor<T1, T2> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2> _computedComponentGroup;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2>(componentDatabase); }
        
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
                    Process(batch.EntityId, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation]);
                });
                return;
            }

            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation]);
            }
        }
    }
    
    public abstract class BatchedSystem<T1, T2, T3> : ManualBatchedSystem
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3));
        
        private readonly BatchPoolAccessor<T1, T2, T3> _batchPoolAccessor;
        private IComputedComponentGroup<T1, T2, T3> _computedComponentGroup;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2, ref T3 component3);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { _batchPoolAccessor = new BatchPoolAccessor<T1, T2, T3>(componentDatabase); }
        
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
                    Process(batch.EntityId, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                        ref components3[batch.Component3Allocation]);
                });
                return;
            }

            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                    ref components3[batch.Component3Allocation]);
            }
        }
    }
}