using EcsR3.Components;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Registries;

namespace EcsR3.Plugins.Batching.Accessors
{
    public interface IBatchManager
    {
        IBatchAccessor<T1,T2> GetAccessorFor<T1, T2>(IComputedEntityGroup observableGroup)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent;
        
        IReferenceBatchAccessor<T1,T2> GetReferenceAccessorFor<T1, T2>(IComputedEntityGroup observableGroup)
            where T1 : class, IComponent
            where T2 : class, IComponent;
        
        IBatchAccessor<T1,T2,T3> GetAccessorFor<T1, T2, T3>(IComputedEntityGroup observableGroup)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent;
        
        IReferenceBatchAccessor<T1,T2,T3> GetReferenceAccessorFor<T1, T2, T3>(IComputedEntityGroup observableGroup)
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent;
        
        IBatchAccessor<T1,T2,T3,T4> GetAccessorFor<T1, T2, T3, T4>(IComputedEntityGroup observableGroup)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent;
        
        IReferenceBatchAccessor<T1,T2,T3,T4> GetReferenceAccessorFor<T1, T2, T3, T4>(IComputedEntityGroup observableGroup)
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent
            where T4 : class, IComponent;
        
        IBatchAccessor<T1,T2,T3,T4,T5> GetAccessorFor<T1, T2, T3, T4, T5>(IComputedEntityGroup observableGroup)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent;
        
        IReferenceBatchAccessor<T1,T2,T3,T4,T5> GetReferenceAccessorFor<T1, T2, T3, T4, T5>(IComputedEntityGroup observableGroup)
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent
            where T4 : class, IComponent
            where T5 : class, IComponent;
        
        IBatchAccessor<T1,T2,T3,T4,T5,T6> GetAccessorFor<T1, T2, T3, T4, T5, T6>(IComputedEntityGroup observableGroup)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent;

        IReferenceBatchAccessor<T1,T2,T3,T4,T5,T6> GetReferenceAccessorFor<T1, T2, T3, T4, T5, T6>(IComputedEntityGroup observableGroup)
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent
            where T4 : class, IComponent
            where T5 : class, IComponent
            where T6 : class, IComponent;


    }
}