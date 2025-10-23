using System;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using SystemsR3.Threading;

namespace EcsR3.Systems.Batching.Convention
{
    public abstract class BatchedRefSystem<T1> : RawBatchedSystem<T1>
        where T1 : struct, IComponent
    {
        protected abstract void Process(Entity entity, ref T1 component1);

        protected BatchedRefSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override void ProcessGroup(ref ReadOnlyMemory<ComponentBatch<T1>> componentBatches, T1[] componentPools)
        {
            if (ShouldMultithread)
            {
                ProcessGroupWithMultithreading(ref componentBatches, componentPools);
                return;
            }

            var batches = componentBatches.Span;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.Entity, ref componentPools[batch.Component1Allocation]);
            }
        }
        
        protected void ProcessGroupWithMultithreading(ref ReadOnlyMemory<ComponentBatch<T1>> componentBatches,
            T1[] componentPools)
        {
            var closureBatches = componentBatches;
            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = closureBatches.Span[i];
                Process(batch.Entity, ref componentPools[batch.Component1Allocation]);
            });
        }
    }
    
    public abstract class BatchedRefSystem<T1, T2> : RawBatchedSystem<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        protected abstract void Process(Entity entity, ref T1 component1, ref T2 component2);

        protected BatchedRefSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {
        }

        protected override void ProcessGroup(ref ReadOnlyMemory<ComponentBatch<T1, T2>> componentBatches, (T1[], T2[]) componentPools)
        {
            if (ShouldMultithread)
            {
                ProcessGroupWithMultithreading(ref componentBatches, componentPools);
                return;
            }

            var (components1, components2) = componentPools;
            var batches = componentBatches.Span;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation]);
            }
        }
        
        protected void ProcessGroupWithMultithreading(ref ReadOnlyMemory<ComponentBatch<T1, T2>> componentBatches,
            (T1[], T2[]) componentPools)
        {
            var (components1, components2) = componentPools;
            var closureBatches = componentBatches;
            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = closureBatches.Span[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation]);
            });
        }
    }
    
    public abstract class BatchedRefSystem<T1, T2, T3> : RawBatchedSystem<T1,T2,T3>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        protected abstract void Process(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3);

        protected BatchedRefSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override void ProcessGroup(ref ReadOnlyMemory<ComponentBatch<T1, T2, T3>> componentBatches, (T1[], T2[], T3[]) componentPools)
        {
            
            if (ShouldMultithread)
            {
                ProcessGroupWithMultithreading(ref componentBatches, componentPools);
                return;
            }

            var (components1, components2, components3) = componentPools;
            var batches = componentBatches.Span;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                    ref components3[batch.Component3Allocation]);
            }
        }
        
        protected void ProcessGroupWithMultithreading(ref ReadOnlyMemory<ComponentBatch<T1, T2, T3>> componentBatches, (T1[], T2[], T3[]) componentPools)
        {
            var (components1, components2, components3) = componentPools;
            var closureBatches = componentBatches;
            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = closureBatches.Span[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                    ref components3[batch.Component3Allocation]);
            });
        }
    }
    
    public abstract class BatchedRefSystem<T1, T2, T3, T4> : RawBatchedSystem<T1,T2,T3, T4>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
    {
        protected abstract void Process(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4);

        protected BatchedRefSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override void ProcessGroup(ref ReadOnlyMemory<ComponentBatch<T1, T2, T3,T4>> componentBatches, (T1[], T2[], T3[], T4[]) componentPools)
        {
            if (ShouldMultithread)
            {
                ProcessGroupWithMultithreading(ref componentBatches, componentPools);
                return;
            }
            
            var (components1, components2, components3, components4) = componentPools;
            var batches = componentBatches.Span;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                    ref components3[batch.Component3Allocation], ref components4[batch.Component4Allocation]);
            }
        }
        
        protected void ProcessGroupWithMultithreading(ref ReadOnlyMemory<ComponentBatch<T1, T2, T3,T4>> componentBatches, (T1[], T2[], T3[], T4[]) componentPools)
        {
            var (components1, components2, components3, components4) = componentPools;
            var closureBatches = componentBatches;
            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = closureBatches.Span[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                    ref components3[batch.Component3Allocation], ref components4[batch.Component4Allocation]);
            });
        }
    }
    
    public abstract class BatchedRefSystem<T1, T2, T3, T4, T5> : RawBatchedSystem<T1,T2,T3, T4, T5>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
    {
        protected abstract void Process(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4, ref T5 component5);

        protected BatchedRefSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override void ProcessGroup(ref ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5>> componentBatches, (T1[], T2[], T3[], T4[], T5[]) componentPools)
        {
            if (ShouldMultithread)
            {
                ProcessGroupWithMultithreading(ref componentBatches, componentPools);
                return;
            }

            var (components1, components2, components3, components4, components5) = componentPools;
            var batches = componentBatches.Span;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                    ref components3[batch.Component3Allocation], ref components4[batch.Component4Allocation],
                    ref components5[batch.Component5Allocation]);
            }
        }
        
        protected void ProcessGroupWithMultithreading(ref ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5>> componentBatches, (T1[], T2[], T3[], T4[], T5[]) componentPools)
        {
            var (components1, components2, components3, components4, components5) = componentPools;
            var closureBatches = componentBatches;
            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = closureBatches.Span[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                    ref components3[batch.Component3Allocation], ref components4[batch.Component4Allocation],
                    ref components5[batch.Component5Allocation]);
            });
        }
    }
    
    public abstract class BatchedRefSystem<T1, T2, T3, T4, T5, T6> : RawBatchedSystem<T1,T2,T3, T4, T5, T6>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
    {
        protected abstract void Process(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4, ref T5 component5, ref T6 component6);

        protected BatchedRefSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override void ProcessGroup(ref ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[]) componentPools)
        {
            if (ShouldMultithread)
            {
                ProcessGroupWithMultithreading(ref componentBatches, componentPools);
                return;
            }
            
            var (components1, components2, components3, components4, components5, components6) = componentPools;
            var batches = componentBatches.Span;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                    ref components3[batch.Component3Allocation], ref components4[batch.Component4Allocation],
                    ref components5[batch.Component5Allocation], ref components6[batch.Component6Allocation]);;;
            }
        }
        
        protected void ProcessGroupWithMultithreading(ref ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[]) componentPools)
        {
            var (components1, components2, components3, components4, components5, components6) = componentPools;
            var closureBatches = componentBatches;
            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = closureBatches.Span[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                    ref components3[batch.Component3Allocation], ref components4[batch.Component4Allocation],
                    ref components5[batch.Component5Allocation], ref components6[batch.Component6Allocation]);
            });
        }
    }
    
    public abstract class BatchedRefSystem<T1, T2, T3, T4, T5, T6, T7> : RawBatchedSystem<T1,T2,T3, T4, T5, T6, T7>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
        where T7 : struct, IComponent
    {
        protected abstract void Process(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4, ref T5 component5, ref T6 component6, ref T7 component7);

        protected BatchedRefSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {
        }

        protected override void ProcessGroup(ref ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6, T7>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[], T7[]) componentPools)
        {
            
            if (ShouldMultithread)
            {
                ProcessGroupWithMultithreading(ref componentBatches, componentPools);
                return;
            }

            var (components1, components2, components3, components4, components5, components6, components7) = componentPools;
            var batches = componentBatches.Span;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                    ref components3[batch.Component3Allocation], ref components4[batch.Component4Allocation],
                    ref components5[batch.Component5Allocation], ref components6[batch.Component6Allocation],
                    ref components7[batch.Component7Allocation]);
            }
        }
        
        protected void ProcessGroupWithMultithreading(ref ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6, T7>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[], T7[]) componentPools)
        {
            var (components1, components2, components3, components4, components5, components6, components7) = componentPools;
            var closureBatches = componentBatches;
            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = closureBatches.Span[i];
                Process(batch.Entity, ref components1[batch.Component1Allocation], ref components2[batch.Component2Allocation],
                    ref components3[batch.Component3Allocation], ref components4[batch.Component4Allocation],
                    ref components5[batch.Component5Allocation], ref components6[batch.Component6Allocation],
                    ref components7[batch.Component7Allocation]);
            });
        }
    }
}