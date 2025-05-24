using EcsR3.Computeds.Entities;
using SystemsR3.Computeds;

namespace EcsR3.Computeds.Components
{
    public interface IComputedComponentGroup : IComputedGroup
    {
        IComputedEntityGroup DataSource { get; }
    }
    
    public interface IComputedComponentGroup<T1> : IComputed<ComponentBatch<T1>[]>, IComputedComponentGroup {}
    public interface IComputedComponentGroup<T1, T2> : IComputed<ComponentBatch<T1, T2>[]>, IComputedComponentGroup {}
    public interface IComputedComponentGroup<T1, T2, T3> : IComputed<ComponentBatch<T1, T2, T3>[]>, IComputedComponentGroup {}
    public interface IComputedComponentGroup<T1, T2, T3, T4> : IComputed<ComponentBatch<T1, T2, T3, T4>[]>, IComputedComponentGroup {}
    public interface IComputedComponentGroup<T1, T2, T3, T4, T5> : IComputed<ComponentBatch<T1, T2, T3, T4, T5>[]>, IComputedComponentGroup {}
    public interface IComputedComponentGroup<T1, T2, T3, T4, T5, T6> : IComputed<ComponentBatch<T1, T2, T3, T4, T5, T6>[]>, IComputedComponentGroup {}
    public interface IComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7> : IComputed<ComponentBatch<T1, T2, T3, T4, T5, T6, T7>[]>, IComputedComponentGroup {}
}