using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Plugins.Batching.Builders;

namespace EcsR3.Plugins.Batching.Factories
{
    public class BatchBuilderFactory : IBatchBuilderFactory
    {
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public BatchBuilderFactory(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
        }

        public IBatchBuilder<T1, T2> Create<T1, T2>() 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent
        { return new BatchBuilder<T1, T2>(ComponentDatabase, ComponentTypeLookup); }

        public IBatchBuilder<T1, T2, T3> Create<T1, T2, T3>() 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent
        {
            return new BatchBuilder<T1, T2, T3>(ComponentDatabase, ComponentTypeLookup);
        }

        public IBatchBuilder<T1, T2, T3, T4> Create<T1, T2, T3, T4>() 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent 
            where T4 : unmanaged, IComponent
        {
            return new BatchBuilder<T1, T2, T3, T4>(ComponentDatabase, ComponentTypeLookup);
        }

        public IBatchBuilder<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>() 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent 
            where T4 : unmanaged, IComponent 
            where T5 : unmanaged, IComponent
        {
            return new BatchBuilder<T1, T2, T3, T4, T5>(ComponentDatabase, ComponentTypeLookup);
        }

        public IBatchBuilder<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>() 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent 
            where T4 : unmanaged, IComponent 
            where T5 : unmanaged, IComponent 
            where T6 : unmanaged, IComponent
        {
            return new BatchBuilder<T1, T2, T3, T4, T5, T6>(ComponentDatabase, ComponentTypeLookup);
        }
    }
}