using System.Collections.Generic;
using EcsR3.Collections;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Computeds.Entities;
using EcsR3.Extensions;
using EcsR3.Plugins.Batching.Factories;

namespace EcsR3.Plugins.Batching.Accessors
{
    public class BatchManager : IBatchManager
    {
        public Dictionary<AccessorToken, IBatchAccessor> BatchAccessors { get; } = new Dictionary<AccessorToken, IBatchAccessor>();
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IBatchBuilderFactory BatchBuilderFactory { get; }
        public IReferenceBatchBuilderFactory ReferenceBatchBuilderFactory { get; }
        public IComputedGroupManager ComputedGroupManager { get; }

        public BatchManager(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IBatchBuilderFactory batchBuilderFactory, IReferenceBatchBuilderFactory referenceBatchBuilderFactory, IComputedGroupManager computedGroupManager)
        {
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
            BatchBuilderFactory = batchBuilderFactory;
            ComputedGroupManager = computedGroupManager;
            ReferenceBatchBuilderFactory = referenceBatchBuilderFactory;
        }

        public IBatchAccessor<T1,T2> GetAccessorFor<T1, T2>(IComputedEntityGroup observableGroup) 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypeIds(typeof(T1), typeof(T2));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.TryGetValue(token, out var accessor))
            { return (IBatchAccessor<T1, T2>) accessor; }

            var batchBuilder = BatchBuilderFactory.Create<T1, T2>();
            var batchAccessor = new BatchAccessor<T1, T2>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IReferenceBatchAccessor<T1,T2> GetReferenceAccessorFor<T1, T2>(IComputedEntityGroup observableGroup) 
            where T1 : class, IComponent 
            where T2 : class, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypeIds(typeof(T1), typeof(T2));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.TryGetValue(token, out var accessor))
            { return (IReferenceBatchAccessor<T1, T2>) accessor; }

            var batchBuilder = ReferenceBatchBuilderFactory.Create<T1, T2>();
            var batchAccessor = new ReferenceBatchAccessor<T1, T2>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IBatchAccessor<T1,T2,T3> GetAccessorFor<T1, T2, T3>(IComputedEntityGroup observableGroup) 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypeIds(typeof(T1), typeof(T2), typeof(T3));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.TryGetValue(token, out var accessor))
            { return (IBatchAccessor<T1, T2, T3>) accessor; }

            var batchBuilder = BatchBuilderFactory.Create<T1, T2, T3>();
            var batchAccessor = new BatchAccessor<T1, T2, T3>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IReferenceBatchAccessor<T1,T2,T3> GetReferenceAccessorFor<T1, T2, T3>(IComputedEntityGroup observableGroup) 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypeIds(typeof(T1), typeof(T2), typeof(T3));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.TryGetValue(token, out var accessor))
            { return (IReferenceBatchAccessor<T1, T2, T3>) accessor; }

            var batchBuilder = ReferenceBatchBuilderFactory.Create<T1, T2, T3>();
            var batchAccessor = new ReferenceBatchAccessor<T1, T2, T3>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IBatchAccessor<T1,T2,T3,T4> GetAccessorFor<T1, T2, T3, T4>(IComputedEntityGroup observableGroup) 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent 
            where T4 : unmanaged, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypeIds(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.TryGetValue(token, out var accessor))
            { return (IBatchAccessor<T1, T2, T3, T4>) accessor; }

            var batchBuilder = BatchBuilderFactory.Create<T1, T2, T3, T4>();
            var batchAccessor = new BatchAccessor<T1, T2, T3, T4>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IReferenceBatchAccessor<T1,T2,T3,T4> GetReferenceAccessorFor<T1, T2, T3, T4>(IComputedEntityGroup observableGroup) 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent 
            where T4 : class, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypeIds(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.TryGetValue(token, out var accessor))
            { return (IReferenceBatchAccessor<T1, T2, T3, T4>) accessor; }

            var batchBuilder = ReferenceBatchBuilderFactory.Create<T1, T2, T3, T4>();
            var batchAccessor = new ReferenceBatchAccessor<T1, T2, T3, T4>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IBatchAccessor<T1, T2, T3, T4, T5> GetAccessorFor<T1, T2, T3, T4, T5>(IComputedEntityGroup observableGroup) 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent 
            where T4 : unmanaged, IComponent 
            where T5 : unmanaged, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypeIds(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.TryGetValue(token, out var accessor))
            { return (IBatchAccessor<T1, T2, T3, T4, T5>) accessor; }

            var batchBuilder = BatchBuilderFactory.Create<T1, T2, T3, T4, T5>();
            var batchAccessor = new BatchAccessor<T1, T2, T3, T4, T5>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IReferenceBatchAccessor<T1, T2, T3, T4, T5> GetReferenceAccessorFor<T1, T2, T3, T4, T5>(IComputedEntityGroup observableGroup) 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent 
            where T4 : class, IComponent 
            where T5 : class, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypeIds(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.TryGetValue(token, out var accessor))
            { return (IReferenceBatchAccessor<T1, T2, T3, T4, T5>) accessor; }

            var batchBuilder = ReferenceBatchBuilderFactory.Create<T1, T2, T3, T4, T5>();
            var batchAccessor = new ReferenceBatchAccessor<T1, T2, T3, T4, T5>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IBatchAccessor<T1, T2, T3, T4, T5,T6> GetAccessorFor<T1, T2, T3, T4, T5, T6>(IComputedEntityGroup observableGroup) 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent 
            where T4 : unmanaged, IComponent 
            where T5 : unmanaged, IComponent 
            where T6 : unmanaged, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypeIds(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.TryGetValue(token, out var accessor))
            { return (IBatchAccessor<T1, T2,T3,T4,T5,T6>) accessor; }

            var batchBuilder = BatchBuilderFactory.Create<T1, T2, T3, T4, T5, T6>();
            var batchAccessor = new BatchAccessor<T1, T2, T3, T4, T5, T6>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IReferenceBatchAccessor<T1, T2, T3, T4, T5, T6> GetReferenceAccessorFor<T1, T2, T3, T4, T5, T6>(IComputedEntityGroup observableGroup) 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent 
            where T4 : class, IComponent 
            where T5 : class, IComponent 
            where T6 : class, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypeIds(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.TryGetValue(token, out var accessor))
            { return (IReferenceBatchAccessor<T1, T2, T3, T4, T5, T6>) accessor; }

            var batchBuilder = ReferenceBatchBuilderFactory.Create<T1, T2, T3, T4, T5, T6>();
            var batchAccessor = new ReferenceBatchAccessor<T1, T2, T3, T4, T5, T6>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }
    }
}