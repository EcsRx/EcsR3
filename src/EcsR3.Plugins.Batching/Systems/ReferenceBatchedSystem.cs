using SystemsR3.Threading;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Groups;

namespace EcsR3.Plugins.Batching.Systems
{
    public abstract class ReferenceBatchedSystem<T1, T2> : ManualBatchedSystem
        where T1 : class, IComponent
        where T2 : class, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2));
        
        private readonly IComponentPool<T1> _componentPool1;
        private readonly IComponentPool<T2> _componentPool2;
        private IComputedComponentGroup<T1, T2> _computedComponentGroup;

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        {
            _componentPool1 = componentDatabase.GetPoolFor<T1>();
            _componentPool2 = componentDatabase.GetPoolFor<T2>();
        }

        protected abstract void Process(int entityId, T1 component1, T2 component2);

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2>();
            return _computedComponentGroup;
        }

        protected override void ProcessBatch()
        {
            var components1 = _componentPool1.Components;
            var components2 = _componentPool2.Components;
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
        
        private readonly IComponentPool<T1> _componentPool1;
        private readonly IComponentPool<T2> _componentPool2;
        private readonly IComponentPool<T3> _componentPool3;
        private IComputedComponentGroup<T1, T2, T3> _computedComponentGroup;

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        {
            _componentPool1 = componentDatabase.GetPoolFor<T1>();
            _componentPool2 = componentDatabase.GetPoolFor<T2>();
            _componentPool3 = componentDatabase.GetPoolFor<T3>();
        }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3);

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2, T3>();
            return _computedComponentGroup;
        }

        protected override void ProcessBatch()
        {
            var components1 = _componentPool1.Components;
            var components2 = _componentPool2.Components;
            var components3 = _componentPool3.Components;
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
        
        private readonly IComponentPool<T1> _componentPool1;
        private readonly IComponentPool<T2> _componentPool2;
        private readonly IComponentPool<T3> _componentPool3;
        private readonly IComponentPool<T4> _componentPool4;
        private IComputedComponentGroup<T1, T2, T3, T4> _computedComponentGroup;

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        {
            _componentPool1 = componentDatabase.GetPoolFor<T1>();
            _componentPool2 = componentDatabase.GetPoolFor<T2>();
            _componentPool3 = componentDatabase.GetPoolFor<T3>();
            _componentPool4 = componentDatabase.GetPoolFor<T4>();
        }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4);

        protected override IComputedComponentGroup GetComponentGroup()
        {
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4>();
            return _computedComponentGroup;
        }

        protected override void ProcessBatch()
        {
            var components1 = _componentPool1.Components;
            var components2 = _componentPool2.Components;
            var components3 = _componentPool3.Components;
            var components4 = _componentPool4.Components;
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
}