using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities.Accessors;
using EcsR3.Systems.Augments;
using EcsR3.Systems.Batching.Convention.Multiplexing.Handlers;
using SystemsR3.Threading;

namespace EcsR3.Systems.Batching.Convention.Multiplexing
{
    public abstract class MultiplexingBatchedSystem<T> : RawBatchedSystem<T> 
        where T : IComponent
    {
        public IMultiplexedJob<T>[] Jobs { get; private set; }
        public ISystemPreProcessor[] PreProcessorJobs { get; private set; }
        public ISystemPostProcessor[] PostProcessorJobs { get; private set; }

        protected abstract IEnumerable<IMultiplexedJob<T>> ResolveJobs();
        
        public MultiplexingBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        public override void StartSystem()
        {
            base.StartSystem();
            Jobs = ResolveJobs().ToArray();
            
            PreProcessorJobs = Jobs.Where(x => x is ISystemPreProcessor)
                .Cast<ISystemPreProcessor>()
                .ToArray();
            
            PostProcessorJobs = Jobs.Where(x => x is ISystemPostProcessor)
                .Cast<ISystemPostProcessor>()
                .ToArray();
        }

        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T>> componentBatches, T[] componentPools)
        {
            for(var i = 0; i < PreProcessorJobs.Length; i++)
            { PreProcessorJobs[i].BeforeProcessing(); }

            if (ShouldMultithread)
            { ProcessJobsMultithreaded( componentBatches, componentPools); }
            else
            { ProcessJobs(componentBatches, componentPools); }
            
            for(var i = 0; i < PostProcessorJobs.Length; i++)
            { PostProcessorJobs[i].AfterProcessing(); }
        }

        protected void ProcessJobsMultithreaded(ReadOnlyMemory<ComponentBatch<T>> componentBatches, T[] componentPools)
        {
            var closureBatches = componentBatches;

            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = closureBatches.Span[i];
                foreach (var job in Jobs)
                { job.Process(batch.Entity, componentPools[batch.Component1Allocation]); }
            });
        }
        
        protected void ProcessJobs(ReadOnlyMemory<ComponentBatch<T>> componentBatches, T[] componentPools)
        {
            var batchesSpan = componentBatches.Span;

            for(var i=0;i<batchesSpan.Length;i++)
            {
                var batch = batchesSpan[i];
                foreach (var job in Jobs)
                { job.Process(batch.Entity, componentPools[batch.Component1Allocation]); }
            }
        }
    }
    
    public abstract class MultiplexingBatchedSystem<T1, T2> : RawBatchedSystem<T1, T2> 
        where T1 : IComponent where T2 : IComponent
    {
        public IMultiplexedJob<T1, T2>[] Jobs { get; private set; }
        public ISystemPreProcessor[] PreProcessorJobs { get; private set; }
        public ISystemPostProcessor[] PostProcessorJobs { get; private set; }

        protected abstract IEnumerable<IMultiplexedJob<T1, T2>> ResolveJobs();
        
        public MultiplexingBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        public override void StartSystem()
        {
            base.StartSystem();
            Jobs = ResolveJobs().ToArray();
            
            PreProcessorJobs = Jobs.Where(x => x is ISystemPreProcessor)
                .Cast<ISystemPreProcessor>()
                .ToArray();
            
            PostProcessorJobs = Jobs.Where(x => x is ISystemPostProcessor)
                .Cast<ISystemPostProcessor>()
                .ToArray();
        }

        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2>> componentBatches, (T1[], T2[]) componentPools)
        {
            foreach (var preProcessor in PreProcessorJobs)
            { preProcessor.BeforeProcessing(); }
            
            if (ShouldMultithread)
            { ProcessJobsMultithreaded(componentBatches, componentPools); }
            else
            { ProcessJobs(componentBatches, componentPools); }
                        
            foreach (var postProcessor in PostProcessorJobs)
            { postProcessor.AfterProcessing(); }
        }
        
        protected void ProcessJobsMultithreaded(ReadOnlyMemory<ComponentBatch<T1, T2>> componentBatches,
            (T1[], T2[]) componentPools)
        {
            var (components1, components2) = componentPools;
            var closureBatches = componentBatches;

            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = closureBatches.Span[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation]);
                }
            });
        }
        
        protected void ProcessJobs(ReadOnlyMemory<ComponentBatch<T1, T2>> componentBatches, (T1[], T2[]) componentPools)
        {
            var (components1, components2) = componentPools;
            var batchesSpan = componentBatches.Span;

            for(var i=0;i<batchesSpan.Length;i++)
            {
                var batch = batchesSpan[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation]);
                }
            }
        }
    }
    
    public abstract class MultiplexingBatchedSystem<T1, T2, T3> : RawBatchedSystem<T1, T2, T3>
        where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        public IMultiplexedJob<T1, T2, T3>[] Jobs { get; private set; }
        public ISystemPreProcessor[] PreProcessorJobs { get; private set; }
        public ISystemPostProcessor[] PostProcessorJobs { get; private set; }

        protected abstract IEnumerable<IMultiplexedJob<T1, T2, T3>> ResolveJobs();
        
        public MultiplexingBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        public override void StartSystem()
        {
            base.StartSystem();
            Jobs = ResolveJobs().ToArray();
            
            PreProcessorJobs = Jobs.Where(x => x is ISystemPreProcessor)
                .Cast<ISystemPreProcessor>()
                .ToArray();
            
            PostProcessorJobs = Jobs.Where(x => x is ISystemPostProcessor)
                .Cast<ISystemPostProcessor>()
                .ToArray();
        }

        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3>> componentBatches, (T1[], T2[], T3[]) componentPools)
        {
            foreach (var preProcessor in PreProcessorJobs)
            { preProcessor.BeforeProcessing(); }

            if (ShouldMultithread)
            { ProcessJobsMultithreaded(componentBatches, componentPools); }
            else
            { ProcessJobs(componentBatches, componentPools); }
            
            foreach (var postProcessor in PostProcessorJobs)
            { postProcessor.AfterProcessing(); }
        }

        protected void ProcessJobsMultithreaded(ReadOnlyMemory<ComponentBatch<T1, T2, T3>> componentBatches,
            (T1[], T2[], T3[]) componentPools)
        {
            var (components1, components2, components3) = componentPools;
            var scopedComponentBatches = componentBatches;

            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = scopedComponentBatches.Span[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation], components3[batch.Component3Allocation]);
                }
            });
        }
        
        protected void ProcessJobs(ReadOnlyMemory<ComponentBatch<T1, T2, T3>> componentBatches, (T1[], T2[], T3[]) componentPools)
        {
            var (components1, components2, components3) = componentPools;
            var batchesSpan = componentBatches.Span;

            for(var i=0;i<batchesSpan.Length;i++)
            {
                var batch = batchesSpan[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation], components3[batch.Component3Allocation]);
                }
            }
        }
    }
    
    public abstract class MultiplexingBatchedSystem<T1, T2, T3, T4> : RawBatchedSystem<T1, T2, T3, T4>
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
    {
        public IMultiplexedJob<T1, T2, T3, T4>[] Jobs { get; private set; }
        public ISystemPreProcessor[] PreProcessorJobs { get; private set; }
        public ISystemPostProcessor[] PostProcessorJobs { get; private set; }

        protected abstract IEnumerable<IMultiplexedJob<T1, T2, T3, T4>> ResolveJobs();
        
        public MultiplexingBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        public override void StartSystem()
        {
            base.StartSystem();
            Jobs = ResolveJobs().ToArray();
            
            PreProcessorJobs = Jobs.Where(x => x is ISystemPreProcessor)
                .Cast<ISystemPreProcessor>()
                .ToArray();
            
            PostProcessorJobs = Jobs.Where(x => x is ISystemPostProcessor)
                .Cast<ISystemPostProcessor>()
                .ToArray();
        }

        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4>> componentBatches, (T1[], T2[], T3[], T4[]) componentPools)
        {
            for(var i = 0; i < PreProcessorJobs.Length; i++)
            { PreProcessorJobs[i].BeforeProcessing(); }
            
            if (ShouldMultithread)
            { ProcessJobsMultithreaded(componentBatches, componentPools); }
            else
            { ProcessJobs(componentBatches, componentPools); }
                        
            for(var i = 0; i < PostProcessorJobs.Length; i++)
            { PostProcessorJobs[i].AfterProcessing(); }
        }

        protected void ProcessJobsMultithreaded(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4>> componentBatches, (T1[], T2[], T3[], T4[]) componentPools)
        {
            var (components1, components2, components3, components4) = componentPools;
            var scopedComponentBatches = componentBatches;

            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = scopedComponentBatches.Span[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation], components3[batch.Component3Allocation],
                        components4[batch.Component4Allocation]);
                }
            });
        }
        
        protected void ProcessJobs(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4>> componentBatches, (T1[], T2[], T3[], T4[]) componentPools)
        {
            var (components1, components2, components3, components4) = componentPools;
            var batchesSpan = componentBatches.Span;

            for(var i=0;i<batchesSpan.Length;i++)
            {
                var batch = batchesSpan[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation], components3[batch.Component3Allocation],
                        components4[batch.Component4Allocation]);
                }
            }
        }
    }    
    
    public abstract class MultiplexingBatchedSystem<T1, T2, T3, T4, T5> : RawBatchedSystem<T1, T2, T3, T4, T5>
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent
    {
        public IMultiplexedJob<T1, T2, T3, T4, T5>[] Jobs { get; private set; }
        public ISystemPreProcessor[] PreProcessorJobs { get; private set; }
        public ISystemPostProcessor[] PostProcessorJobs { get; private set; }

        protected abstract IEnumerable<IMultiplexedJob<T1, T2, T3, T4, T5>> ResolveJobs();
        
        public MultiplexingBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        public override void StartSystem()
        {
            base.StartSystem();
            Jobs = ResolveJobs().ToArray();
            
            PreProcessorJobs = Jobs.Where(x => x is ISystemPreProcessor)
                .Cast<ISystemPreProcessor>()
                .ToArray();
            
            PostProcessorJobs = Jobs.Where(x => x is ISystemPostProcessor)
                .Cast<ISystemPostProcessor>()
                .ToArray();
        }

        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5>> componentBatches, (T1[], T2[], T3[], T4[], T5[]) componentPools)
        {
            for(var i = 0; i < PreProcessorJobs.Length; i++)
            { PreProcessorJobs[i].BeforeProcessing(); }
            
            if (ShouldMultithread)
            { ProcessJobsMultithreaded(componentBatches, componentPools); }
            else
            { ProcessJobs(componentBatches, componentPools); }
                        
            for(var i = 0; i < PostProcessorJobs.Length; i++)
            { PostProcessorJobs[i].AfterProcessing(); }
        }
       
        protected void ProcessJobsMultithreaded(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5>> componentBatches, (T1[], T2[], T3[], T4[], T5[]) componentPools)
        {
            var (components1, components2, components3, components4, components5) = componentPools;
            var scopedComponentBatches = componentBatches;

            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = scopedComponentBatches.Span[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation], components3[batch.Component3Allocation],
                        components4[batch.Component4Allocation], components5[batch.Component5Allocation]);
                }
            });
        }
        
        protected void ProcessJobs(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5>> componentBatches, (T1[], T2[], T3[], T4[], T5[]) componentPools)
        {
            var (components1, components2, components3, components4, components5) = componentPools;
            var batchesSpan = componentBatches.Span;

            for(var i=0;i<batchesSpan.Length;i++)
            {
                var batch = batchesSpan[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation], components3[batch.Component3Allocation],
                        components4[batch.Component4Allocation], components5[batch.Component5Allocation]);
                }
            }
        }
    }
    
    public abstract class MultiplexingBatchedSystem<T1, T2, T3, T4, T5, T6> : RawBatchedSystem<T1, T2, T3, T4, T5, T6>
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent
    {
        public IMultiplexedJob<T1, T2, T3, T4, T5, T6>[] Jobs { get; private set; }
        public ISystemPreProcessor[] PreProcessorJobs { get; private set; }
        public ISystemPostProcessor[] PostProcessorJobs { get; private set; }

        protected abstract IEnumerable<IMultiplexedJob<T1, T2, T3, T4, T5, T6>> ResolveJobs();
        
        public MultiplexingBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        public override void StartSystem()
        {
            base.StartSystem();
            Jobs = ResolveJobs().ToArray();
            
            PreProcessorJobs = Jobs.Where(x => x is ISystemPreProcessor)
                .Cast<ISystemPreProcessor>()
                .ToArray();
            
            PostProcessorJobs = Jobs.Where(x => x is ISystemPostProcessor)
                .Cast<ISystemPostProcessor>()
                .ToArray();
        }

        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[]) componentPools)
        {
            for(var i = 0; i < PreProcessorJobs.Length; i++)
            { PreProcessorJobs[i].BeforeProcessing(); }
            
            if (ShouldMultithread)
            { ProcessJobsMultithreaded(componentBatches,  componentPools); }
            else
            { ProcessJobs(componentBatches, componentPools); }
                        
            for(var i = 0; i < PostProcessorJobs.Length; i++)
            { PostProcessorJobs[i].AfterProcessing(); }
        }
        
        protected void ProcessJobsMultithreaded(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[]) componentPools)
        {
            var (components1, components2, components3, components4, components5, components6) = componentPools;
            var scopedComponentBatches = componentBatches;

            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = scopedComponentBatches.Span[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation], components3[batch.Component3Allocation],
                        components4[batch.Component4Allocation], components5[batch.Component5Allocation],
                        components6[batch.Component6Allocation]);
                }
            });
        }
        
        protected void ProcessJobs(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[]) componentPools)
        {
            var (components1, components2, components3, components4, components5, components6) = componentPools;
            var batchesSpan = componentBatches.Span;

            for(var i=0;i<batchesSpan.Length;i++)
            {
                var batch = batchesSpan[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation], components3[batch.Component3Allocation],
                        components4[batch.Component4Allocation], components5[batch.Component5Allocation],
                        components6[batch.Component6Allocation]);
                }
            }
        }
    }
    
    public abstract class MultiplexingBatchedSystem<T1, T2, T3, T4, T5, T6, T7> : RawBatchedSystem<T1, T2, T3, T4, T5, T6, T7>
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent where T7 : IComponent
    {
        public IMultiplexedJob<T1, T2, T3, T4, T5, T6, T7>[] Jobs { get; private set; }
        public ISystemPreProcessor[] PreProcessorJobs { get; private set; }
        public ISystemPostProcessor[] PostProcessorJobs { get; private set; }

        protected abstract IEnumerable<IMultiplexedJob<T1, T2, T3, T4, T5, T6, T7>> ResolveJobs();
        
        public MultiplexingBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        public override void StartSystem()
        {
            base.StartSystem();
            Jobs = ResolveJobs().ToArray();
            
            PreProcessorJobs = Jobs.Where(x => x is ISystemPreProcessor)
                .Cast<ISystemPreProcessor>()
                .ToArray();
            
            PostProcessorJobs = Jobs.Where(x => x is ISystemPostProcessor)
                .Cast<ISystemPostProcessor>()
                .ToArray();
        }

        protected override void ProcessGroup(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6, T7>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[], T7[]) componentPools)
        {
            for(var i = 0; i < PreProcessorJobs.Length; i++)
            { PreProcessorJobs[i].BeforeProcessing(); }
            
            if (ShouldMultithread)
            { ProcessJobsMultithreaded(componentBatches, componentPools); }
            else
            { ProcessJobs(componentBatches, componentPools); }
                        
            for(var i = 0; i < PostProcessorJobs.Length; i++)
            { PostProcessorJobs[i].AfterProcessing(); }
        }
        
        protected void ProcessJobsMultithreaded(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6, T7>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[], T7[]) componentPools)
        {
            var (components1, components2, components3, components4, components5, components6, components7) = componentPools;
            var scopedComponentBatches = componentBatches;

            ThreadHandler.For(0, componentBatches.Length, i =>
            {
                var batch = scopedComponentBatches.Span[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation], components3[batch.Component3Allocation],
                        components4[batch.Component4Allocation], components5[batch.Component5Allocation],
                        components6[batch.Component6Allocation], components7[batch.Component7Allocation]);
                }
            });
        }
        
        protected void ProcessJobs(ReadOnlyMemory<ComponentBatch<T1, T2, T3, T4, T5, T6, T7>> componentBatches, (T1[], T2[], T3[], T4[], T5[], T6[], T7[]) componentPools)
        {
            var (components1, components2, components3, components4, components5, components6, components7) = componentPools;
            var batchesSpan = componentBatches.Span;

            for(var i=0;i<batchesSpan.Length;i++)
            {
                var batch = batchesSpan[i];
                foreach (var job in Jobs)
                {
                    job.Process(batch.Entity, components1[batch.Component1Allocation],
                        components2[batch.Component2Allocation], components3[batch.Component3Allocation],
                        components4[batch.Component4Allocation], components5[batch.Component5Allocation],
                        components6[batch.Component6Allocation], components7[batch.Component7Allocation]);
                }
            }
        }
    }
}