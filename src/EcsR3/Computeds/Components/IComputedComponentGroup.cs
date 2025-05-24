using EcsR3.Components;
using EcsR3.Computeds.Entities;
using SystemsR3.Computeds;

namespace EcsR3.Computeds.Components
{
    public interface IComputedComponentGroup : IComputedGroup
    {
        IComputedEntityGroup DataSource { get; }
        void RefreshData();
    }
    
    public interface IComputedComponentGroup<T1> : IComputed<ComponentBatch<T1>[]>, IComputedComponentGroup where T1 : IComponent {}
    public interface IComputedComponentGroup<T1, T2> : IComputed<ComponentBatch<T1, T2>[]>, IComputedComponentGroup where T1 : IComponent where T2 : IComponent {}
    public interface IComputedComponentGroup<T1, T2, T3> : IComputed<ComponentBatch<T1, T2, T3>[]>, IComputedComponentGroup where T1 : IComponent where T2 : IComponent where T3 : IComponent {}
    public interface IComputedComponentGroup<T1, T2, T3, T4> : IComputed<ComponentBatch<T1, T2, T3, T4>[]>, IComputedComponentGroup  where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent {}
    public interface IComputedComponentGroup<T1, T2, T3, T4, T5> : IComputed<ComponentBatch<T1, T2, T3, T4, T5>[]>, IComputedComponentGroup  where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent {}
    public interface IComputedComponentGroup<T1, T2, T3, T4, T5, T6> : IComputed<ComponentBatch<T1, T2, T3, T4, T5, T6>[]>, IComputedComponentGroup  where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent {}
    public interface IComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7> : IComputed<ComponentBatch<T1, T2, T3, T4, T5, T6, T7>[]>, IComputedComponentGroup where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent where T7 : IComponent {}
}