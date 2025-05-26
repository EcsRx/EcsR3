using System;
using EcsR3.Components;
using EcsR3.Components.Database;
using R3;
using SystemsR3.Computeds.Conventions;

namespace EcsR3.Computeds.Components.Conventions
{
    public abstract class ComputedFromComponentGroup<TOutput, T1> : ComputedFromData<TOutput, IComputedComponentGroup<T1>> where T1 : IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        protected (int, T1)[] BatchCache = Array.Empty<(int, T1)>();

        protected readonly IComponentPool<T1> ComponentPool1;
        
        protected ComputedFromComponentGroup(IComponentDatabase componentDatabase,
            IComputedComponentGroup<T1> dataSource) : base(dataSource)
        {
            ComponentDatabase = componentDatabase;
            ComponentPool1 = componentDatabase.GetPoolFor<T1>();
        }

        protected override Observable<Unit> RefreshWhen() => DataSource.OnChanged.Select(x => Unit.Default);

        protected abstract void UpdateComputedData(ReadOnlyMemory<(int, T1)> componentData);
        
        protected override void UpdateComputedData()
        {
            var components1 = ComponentPool1.Components;
            var batches = DataSource.Value.Span;
            
            if(BatchCache.Length < batches.Length)
            { Array.Resize(ref BatchCache, batches.Length); }

            var usedIndexes = 0;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                BatchCache[usedIndexes++] = (batch.EntityId, components1[batch.Component1Allocation]);
            }
            
            UpdateComputedData(new ReadOnlyMemory<(int, T1)>(BatchCache, 0, usedIndexes));
        }
    }
    
    public abstract class ComputedDataFromComponentGroup<TOutput, T1, T2> : ComputedFromData<TOutput, IComputedComponentGroup<T1, T2>> 
        where T1 : IComponent where T2 : IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        protected (int, T1, T2)[] BatchCache = Array.Empty<(int, T1, T2)>();

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

        protected abstract void UpdateComputedData(ReadOnlyMemory<(int, T1, T2)> componentData);
        
        protected override void UpdateComputedData()
        {
            var components1 = ComponentPool1.Components;
            var components2 = ComponentPool2.Components;
            var batches = DataSource.Value.Span;
            
            if(BatchCache.Length < batches.Length)
            { Array.Resize(ref BatchCache, batches.Length); }

            var usedIndexes = 0;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                BatchCache[usedIndexes++] = (batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation]);
            }
            
            UpdateComputedData(new ReadOnlyMemory<(int, T1, T2)>(BatchCache, 0, usedIndexes));
        }
    }
    
    public abstract class ComputedDataFromComponentGroup<TOutput, T1, T2, T3> : ComputedFromData<TOutput, IComputedComponentGroup<T1, T2, T3>> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }

        protected readonly IComponentPool<T1> ComponentPool1;
        protected readonly IComponentPool<T2> ComponentPool2;
        protected readonly IComponentPool<T3> ComponentPool3;
        protected (int, T1, T2,T3)[] BatchCache = Array.Empty<(int, T1, T2,T3)>();
        
        protected ComputedDataFromComponentGroup(IComponentDatabase componentDatabase,
            IComputedComponentGroup<T1, T2, T3> dataSource) : base(dataSource)
        {
            ComponentDatabase = componentDatabase;
            ComponentPool1 = componentDatabase.GetPoolFor<T1>();
            ComponentPool2 = componentDatabase.GetPoolFor<T2>();
            ComponentPool3 = componentDatabase.GetPoolFor<T3>();
        }

        protected override Observable<Unit> RefreshWhen() => DataSource.OnChanged.Select(x => Unit.Default);

        protected abstract void UpdateComputedData(ReadOnlyMemory<(int, T1, T2, T3)> componentData);
        
        protected override void UpdateComputedData()
        {
            var components1 = ComponentPool1.Components;
            var components2 = ComponentPool2.Components;
            var components3 = ComponentPool3.Components;
            var batches = DataSource.Value.Span;
            
            if(BatchCache.Length < batches.Length)
            { Array.Resize(ref BatchCache, batches.Length); }

            var usedIndexes = 0;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                BatchCache[usedIndexes++] = (batch.EntityId, components1[batch.Component1Allocation], components2[batch.Component2Allocation],
                        components3[batch.Component3Allocation]);
            }
            
            UpdateComputedData(new ReadOnlyMemory<(int, T1, T2, T3)>(BatchCache, 0, usedIndexes));
        }
    }
}