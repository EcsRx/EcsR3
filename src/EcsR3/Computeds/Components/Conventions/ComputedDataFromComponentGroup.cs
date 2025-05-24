using System;
using EcsR3.Components;
using EcsR3.Components.Database;
using R3;
using SystemsR3.Computeds.Conventions;

namespace EcsR3.Computeds.Components.Conventions
{
    public abstract class ComputedDataFromComponentGroup<TOutput, T1> : ComputedFromData<TOutput, IComputedComponentGroup<T1>> where T1 : IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }

        protected readonly IComponentPool<T1> ComponentPool1;
        
        protected ComputedDataFromComponentGroup(IComponentDatabase componentDatabase,
            IComputedComponentGroup<T1> dataSource) : base(dataSource)
        {
            ComponentDatabase = componentDatabase;
            ComponentPool1 = componentDatabase.GetPoolFor<T1>();
        }

        protected override Observable<Unit> RefreshWhen() => DataSource.OnChanged.Select(x => Unit.Default);

        protected override void UpdateComputedData()
        {
            var components1 = ComponentPool1.Components;
            
            var batches = DataSource.Value;
            var batchLookup = new Memory<(int, T1)>(new (int, T1)[batches.Length]);
            var batchLookupSpan = batchLookup.Span;

            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                batchLookupSpan[i] = (batch.EntityId, components1[batch.Component1Allocation]);
            }
            
            UpdateComputedData(batchLookup);
        }
        
        protected abstract void UpdateComputedData(Memory<(int, T1)> componentData);
    }
    
    public abstract class ComputedDataFromComponentGroup<TOutput, T1, T2> : ComputedFromData<TOutput, IComputedComponentGroup<T1, T2>> 
        where T1 : IComponent where T2 : IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }

        protected readonly IComponentPool<T1> ComponentPool1;
        protected readonly IComponentPool<T2> ComponentPool2;
        
        protected ComputedDataFromComponentGroup(IComponentDatabase componentDatabase,
            IComputedComponentGroup<T1, T2> dataSource) : base(dataSource)
        {
            ComponentDatabase = componentDatabase;
            ComponentPool1 = componentDatabase.GetPoolFor<T1>();
            ComponentPool2 = componentDatabase.GetPoolFor<T2>();
        }

        protected override Observable<Unit> RefreshWhen() => DataSource.OnChanged.Select(x => Unit.Default);

        protected override void UpdateComputedData()
        {
            var components1 = ComponentPool1.Components;
            var components2 = ComponentPool2.Components;
            
            var batches = DataSource.Value;
            var batchLookup = new Memory<(int, T1, T2)>(new (int, T1, T2)[batches.Length]);
            var batchLookupSpan = batchLookup.Span;

            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                batchLookupSpan[i] = (batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation]);
            }
            
            UpdateComputedData(batchLookup);
        }
        
        protected abstract void UpdateComputedData(Memory<(int, T1, T2)> componentData);
    }
    
    public abstract class ComputedDataFromComponentGroup<TOutput, T1, T2, T3> : ComputedFromData<TOutput, IComputedComponentGroup<T1, T2, T3>> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }

        protected readonly IComponentPool<T1> ComponentPool1;
        protected readonly IComponentPool<T2> ComponentPool2;
        protected readonly IComponentPool<T3> ComponentPool3;
        
        protected ComputedDataFromComponentGroup(IComponentDatabase componentDatabase,
            IComputedComponentGroup<T1, T2, T3> dataSource) : base(dataSource)
        {
            ComponentDatabase = componentDatabase;
            ComponentPool1 = componentDatabase.GetPoolFor<T1>();
            ComponentPool2 = componentDatabase.GetPoolFor<T2>();
            ComponentPool3 = componentDatabase.GetPoolFor<T3>();
        }

        protected override Observable<Unit> RefreshWhen() => DataSource.OnChanged.Select(x => Unit.Default);

        protected override void UpdateComputedData()
        {
            var components1 = ComponentPool1.Components;
            var components2 = ComponentPool2.Components;
            var components3 = ComponentPool3.Components;
            
            var batches = DataSource.Value;
            var batchLookup = new Memory<(int, T1, T2, T3)>(new (int, T1, T2, T3)[batches.Length]);
            var batchLookupSpan = batchLookup.Span;

            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                batchLookupSpan[i] = (batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation], components3[batch.Component3Allocation]);
            }
            
            UpdateComputedData(batchLookup);
        }
        
        protected abstract void UpdateComputedData(Memory<(int, T1, T2, T3)> componentData);
    }
}