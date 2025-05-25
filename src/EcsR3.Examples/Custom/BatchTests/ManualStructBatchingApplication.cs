using System.Linq;
using System.Numerics;
using EcsR3.Components;
using EcsR3.Computeds.Components;
using EcsR3.Entities;
using EcsR3.Examples.Custom.BatchTests.Components;
using EcsR3.Examples.ExampleApps.Playground;

namespace EcsR3.Examples.Custom.BatchTests
{
    public class ManualStructBatchingApplication : BasicLoopApplication
    {
        private IComputedComponentGroup<StructComponent, StructComponent2> _computedComponentGroup;
        private IComponentPool<StructComponent> _structComponentPool;
        private IComponentPool<StructComponent2> _structComponent2Pool;
        
        protected override void SetupEntities()
        {
            _structComponentPool = ComponentDatabase.GetPoolFor<StructComponent>();
            _structComponentPool.Expand(EntityCount);
            
            _structComponent2Pool = ComponentDatabase.GetPoolFor<StructComponent2>();
            _structComponent2Pool.Expand(EntityCount);
            
            base.SetupEntities();
            
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<StructComponent, StructComponent2>();
            _computedComponentGroup.RefreshData();
        }

        protected override string Description { get; } = "Uses auto batching to group components for cached lookups and quicker reads/writes";

        protected override void SetupEntity(IEntity entity)
        {
            entity.CreateComponent<StructComponent>();
            entity.CreateComponent<StructComponent2>();
        }

        protected override void RunProcess()
        {
            var components1 = _structComponentPool.Components;
            var components2 = _structComponent2Pool.Components;
            var batches = _computedComponentGroup.Value;
            for (var i = batches.Length - 1; i >= 0; i--)
                unsafe
                {
                    var batch = batches[i];
                    ref var basic = ref components1[batch.Component1Allocation];
                    ref var basic2 = ref components2[batch.Component2Allocation];
                    basic.Position += Vector3.One;
                    basic.Something += 10;
                    basic2.IsTrue = 1;
                    basic2.Value += 10;
                }
        }
    }
}