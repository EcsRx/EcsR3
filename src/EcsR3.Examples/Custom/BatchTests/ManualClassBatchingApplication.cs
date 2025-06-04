using System.Numerics;
using EcsR3.Components;
using EcsR3.Computeds.Components;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Application;
using EcsR3.Examples.Custom.BatchTests.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.BatchTests
{
    public class ManualClassBatchingApplication : BasicLoopApplication
    {
        private IComputedComponentGroup<ClassComponent, ClassComponent2> _computedComponentGroup;
        private IComponentPool<ClassComponent> _classComponentPool;
        private IComponentPool<ClassComponent2> _classComponent2Pool;
        
        protected override void SetupEntities()
        {
            _classComponentPool = ComponentDatabase.GetPoolFor<ClassComponent>();
            _classComponentPool.Expand(EntityCount);
            
            _classComponent2Pool = ComponentDatabase.GetPoolFor<ClassComponent2>();
            _classComponent2Pool.Expand(EntityCount);
            
            base.SetupEntities();
            
            _computedComponentGroup = ComputedComponentGroupRegistry.GetComputedGroup<ClassComponent, ClassComponent2>();
            _computedComponentGroup.RefreshData();
        }

        protected override string Description { get; } = "Uses auto batching to allow the components to be clustered better in memory";

        protected override void SetupEntity(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            entityComponentAccessor.CreateComponent<ClassComponent>(entity);
            entityComponentAccessor.CreateComponent<ClassComponent2>(entity);
        }

        protected override void RunProcess()
        {
            var components1 = _classComponentPool.Components;
            var components2 = _classComponent2Pool.Components;
            var batches = _computedComponentGroup.Value.Span;
            for (var i = batches.Length - 1; i >= 0; i--)
            {
                var batch = batches[i];
                var basic = components1[batch.Component1Allocation];
                var basic2 = components2[batch.Component2Allocation];
                basic.Position += Vector3.One;
                basic.Something += 10;
                basic2.IsTrue = true;
                basic2.Value += 10;
            }
        }
    }
}