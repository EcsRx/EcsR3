using System;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Entities;
using R3;
using SystemsR3.Computeds.Conventions;

namespace EcsR3.Computeds.Components.Conventions
{
    public abstract class ComputedFromComponentGroup<TOutput, T1> : ComputedFromData<TOutput, IComputedComponentGroup<T1>> where T1 : IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        protected (Entity, T1)[] BatchCache = Array.Empty<(Entity, T1)>();

        protected readonly IComponentPool<T1> ComponentPool1;
        
        protected ComputedFromComponentGroup(IComponentDatabase componentDatabase,
            IComputedComponentGroup<T1> dataSource) : base(dataSource)
        {
            ComponentDatabase = componentDatabase;
            ComponentPool1 = componentDatabase.GetPoolFor<T1>();
        }

        protected override Observable<Unit> RefreshWhen() => DataSource.OnChanged.Select(x => Unit.Default);

        protected abstract void UpdateComputedData(ReadOnlyMemory<(Entity, T1)> componentData);
        
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
                BatchCache[usedIndexes++] = (batch.Entity, components1[batch.Component1Allocation]);
            }
            
            UpdateComputedData(new ReadOnlyMemory<(Entity, T1)>(BatchCache, 0, usedIndexes));
        }
    }
    
    public abstract class ComputedFromComponentGroup<TOutput, T1, T2> : ComputedFromData<TOutput, IComputedComponentGroup<T1, T2>> 
        where T1 : IComponent where T2 : IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        protected (Entity, T1, T2)[] BatchCache = Array.Empty<(Entity, T1, T2)>();

        protected readonly IComponentPool<T1> ComponentPool1;
        protected readonly IComponentPool<T2> ComponentPool2;
        
        protected ComputedFromComponentGroup(IComponentDatabase componentDatabase,
            IComputedComponentGroup<T1, T2> dataSource) : base(dataSource)
        {
            ComponentDatabase = componentDatabase;
            ComponentPool1 = componentDatabase.GetPoolFor<T1>();
            ComponentPool2 = componentDatabase.GetPoolFor<T2>();
        }

        protected override Observable<Unit> RefreshWhen() => DataSource.OnChanged.Select(x => Unit.Default);

        protected abstract void UpdateComputedData(ReadOnlyMemory<(Entity, T1, T2)> componentData);
        
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
                BatchCache[usedIndexes++] = (batch.Entity, components1[batch.Component1Allocation], components2[batch.Component2Allocation]);
            }
            
            UpdateComputedData(new ReadOnlyMemory<(Entity, T1, T2)>(BatchCache, 0, usedIndexes));
        }
    }
    
    public abstract class ComputedFromComponentGroup<TOutput, T1, T2, T3> : ComputedFromData<TOutput, IComputedComponentGroup<T1, T2, T3>> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }

        protected readonly IComponentPool<T1> ComponentPool1;
        protected readonly IComponentPool<T2> ComponentPool2;
        protected readonly IComponentPool<T3> ComponentPool3;
        protected (Entity, T1, T2,T3)[] BatchCache = Array.Empty<(Entity, T1, T2,T3)>();
        
        protected ComputedFromComponentGroup(IComponentDatabase componentDatabase,
            IComputedComponentGroup<T1, T2, T3> dataSource) : base(dataSource)
        {
            ComponentDatabase = componentDatabase;
            ComponentPool1 = componentDatabase.GetPoolFor<T1>();
            ComponentPool2 = componentDatabase.GetPoolFor<T2>();
            ComponentPool3 = componentDatabase.GetPoolFor<T3>();
        }

        protected override Observable<Unit> RefreshWhen() => DataSource.OnChanged.Select(x => Unit.Default);

        protected abstract void UpdateComputedData(ReadOnlyMemory<(Entity, T1, T2, T3)> componentData);
        
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
                BatchCache[usedIndexes++] = (batch.Entity, components1[batch.Component1Allocation], components2[batch.Component2Allocation],
                        components3[batch.Component3Allocation]);
            }
            
            UpdateComputedData(new ReadOnlyMemory<(Entity, T1, T2, T3)>(BatchCache, 0, usedIndexes));
        }
    }
    
        public abstract class ComputedFromComponentGroup<TOutput, T1, T2, T3, T4> : ComputedFromData<TOutput, IComputedComponentGroup<T1, T2, T3, T4>> 
            where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
        {
        public IComponentDatabase ComponentDatabase { get; }

        protected readonly IComponentPool<T1> ComponentPool1;
        protected readonly IComponentPool<T2> ComponentPool2;
        protected readonly IComponentPool<T3> ComponentPool3;
        protected readonly IComponentPool<T4> ComponentPool4;
        protected (Entity, T1, T2,T3,T4)[] BatchCache = Array.Empty<(Entity, T1, T2,T3,T4)>();
        
        protected ComputedFromComponentGroup(IComponentDatabase componentDatabase,
            IComputedComponentGroup<T1, T2, T3, T4> dataSource) : base(dataSource)
        {
            ComponentDatabase = componentDatabase;
            ComponentPool1 = componentDatabase.GetPoolFor<T1>();
            ComponentPool2 = componentDatabase.GetPoolFor<T2>();
            ComponentPool3 = componentDatabase.GetPoolFor<T3>();
            ComponentPool4 = componentDatabase.GetPoolFor<T4>();
        }

        protected override Observable<Unit> RefreshWhen() => DataSource.OnChanged.Select(x => Unit.Default);

        protected abstract void UpdateComputedData(ReadOnlyMemory<(Entity, T1, T2, T3, T4)> componentData);
        
        protected override void UpdateComputedData()
        {
            var components1 = ComponentPool1.Components;
            var components2 = ComponentPool2.Components;
            var components3 = ComponentPool3.Components;
            var components4 = ComponentPool4.Components;
            var batches = DataSource.Value.Span;
            
            if(BatchCache.Length < batches.Length)
            { Array.Resize(ref BatchCache, batches.Length); }

            var usedIndexes = 0;
            for (var i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                BatchCache[usedIndexes++] = (batch.Entity, components1[batch.Component1Allocation], components2[batch.Component2Allocation],
                        components3[batch.Component3Allocation], components4[batch.Component4Allocation]);;
            }
            
            UpdateComputedData(new ReadOnlyMemory<(Entity, T1, T2, T3, T4)>(BatchCache, 0, usedIndexes));
        }
    }
}