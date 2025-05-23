using SystemsR3.Computeds;

namespace EcsR3.Computeds.Components
{
    public interface IComputedComponentGroup<T1> : IComputed<ComponentBatch<T1>[]>, IComputedGroup {}
    public interface IComputedComponentGroup<T1, T2> : IComputed<ComponentBatch<T1, T2>[]>, IComputedGroup {}
    public interface IComputedComponentGroup<T1, T2, T3> : IComputed<ComponentBatch<T1, T2, T3>[]>, IComputedGroup {}
    public interface IComputedComponentGroup<T1, T2, T3, T4> : IComputed<ComponentBatch<T1, T2, T3, T4>[]>, IComputedGroup {}
}