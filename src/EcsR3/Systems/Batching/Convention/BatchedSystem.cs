using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Registries;
using SystemsR3.Threading;

namespace EcsR3.Systems.Batching.Convention
{
    public abstract class BatchedSystem<T1, T2> : RawBatchedSystem<T1, T2>
        where T1 : IComponent
        where T2 : IComponent
    {
        protected BatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { }

        protected abstract void Process(int entityId, T1 component1, T2 component2);

        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2>> componentBatches, (T1[], T2[]) componentPools)
        {
            var (components1, components2) = componentPools;
            if (ShouldMultithread)
            {
                // TODO: This *SHOULD* be faster, but its not, so maybe investigate this in the future
                /*
                Parallel.ForEach(Partitioner.Create(0, componentBatches.Length), item =>
                {
                    var batches = componentBatches.Span;
                    for (var i = item.Item1; i < item.Item2; i++)
                    {
                        var batch = batches[i];
                        Process(batch.EntityId, components1[batch.Component1Allocation],
                            components2[batch.Component2Allocation]);
                    }
                });*/
            
                ThreadHandler.For(0, componentBatches.Length, i =>
                {
                    var batch = componentBatches.Span[i];
                    Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation]);
                });
                
                return;
            }

            var batches = componentBatches.Span;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation]);
            }
        }
    }
    
    public abstract class BatchedSystem<T1, T2, T3> : RawBatchedSystem<T1, T2, T3>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        protected BatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3);

        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3>> componentBatches, (T1[], T2[], T3[]) componentPools)
        {
            var (components1, components2, components3) = componentPools;
            if (ShouldMultithread)
            {
                ThreadHandler.For(0, componentBatches.Length, i =>
                {
                    var batch = componentBatches.Span[i];
                    Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], components3[batch.Component3Allocation]);
                });
                return;
            }

            var batches = componentBatches.Span;
            for (var i = 0; i < componentBatches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], components3[batch.Component3Allocation]);
            }
        }
    }
    
    public abstract class BatchedSystem<T1, T2, T3, T4> : RawBatchedSystem<T1, T2, T3, T4>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
    {
        protected BatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4);
        
        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4>> componentBatches, (T1[], T2[], T3[], T4[]) componentPools)
        {
            var (components1, components2, components3, components4) = componentPools;
            if (ShouldMultithread)
            {
                ThreadHandler.For(0, componentBatches.Length, i =>
                {
                    var batch = componentBatches.Span[i];
                    Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                        components3[batch.Component3Allocation], components4[batch.Component4Allocation]);;
                });
                return;
            }
            
            var batches = componentBatches.Span;
            for (var i = 0; i < componentBatches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                    components3[batch.Component3Allocation], components4[batch.Component4Allocation]);;
            }
        }
    }
    
    public abstract class BatchedSystem<T1, T2, T3, T4, T5> : RawBatchedSystem<T1, T2, T3, T4, T5>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
    {
        protected BatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        {  }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5);

        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5>> componentBatches, (T1[], T2[], T3[], T4[], T5[]) componentPools)
        {
            var (components1, components2, components3, components4, components5) = componentPools;
            if (ShouldMultithread)
            {
                ThreadHandler.For(0, componentBatches.Length, i =>
                {
                    var batch = componentBatches.Span[i];
                    Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                        components3[batch.Component3Allocation], components4[batch.Component4Allocation],
                        components5[batch.Component5Allocation]);
                });
                return;
            }
            
            var batches = componentBatches.Span;
            for (var i = 0; i < componentBatches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                    components3[batch.Component3Allocation], components4[batch.Component4Allocation],
                    components5[batch.Component5Allocation]);
            }
        }
    }
    
    public abstract class BatchedSystem<T1, T2, T3, T4, T5, T6> : RawBatchedSystem<T1, T2, T3, T4, T5, T6>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
    {
        protected BatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6);
        
        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[]) componentPools)
        {
            var (components1, components2, components3, components4, components5, components6) = componentPools;
            if (ShouldMultithread)
            {
                ThreadHandler.For(0, componentBatches.Length, i =>
                {
                    var batch = componentBatches.Span[i];
                    Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                        components3[batch.Component3Allocation], components4[batch.Component4Allocation],
                        components5[batch.Component5Allocation], components6[batch.Component6Allocation]);
                });
                return;
            }

            var batches = componentBatches.Span;
            for (var i = 0; i < componentBatches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                    components3[batch.Component3Allocation], components4[batch.Component4Allocation],
                    components5[batch.Component5Allocation], components6[batch.Component6Allocation]);
            }
        }
    }
    
    public abstract class BatchedSystem<T1, T2, T3, T4, T5, T6, T7> : RawBatchedSystem<T1, T2, T3, T4, T5, T6, T7>
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
        where T7 : IComponent
    {
        protected BatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        { }

        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6, T7 component7);
        
        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6, T7>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[], T7[]) componentPools)
        {
            var (components1, components2, components3, components4, components5, components6, components7) = componentPools;
            if (ShouldMultithread)
            {
                ThreadHandler.For(0, componentBatches.Length, i =>
                {
                    var batch = componentBatches.Span[i];
                    Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                        components3[batch.Component3Allocation], components4[batch.Component4Allocation],
                        components5[batch.Component5Allocation], components6[batch.Component6Allocation],
                        components7[batch.Component7Allocation]);
                });
                return;
            }

            var batches = componentBatches.Span;
            for (var i = 0; i < componentBatches.Length; i++)
            {
                var batch = batches[i];
                Process(batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], 
                    components3[batch.Component3Allocation], components4[batch.Component4Allocation],
                    components5[batch.Component5Allocation], components6[batch.Component6Allocation],
                    components7[batch.Component7Allocation]);
            }
        }
    }
}