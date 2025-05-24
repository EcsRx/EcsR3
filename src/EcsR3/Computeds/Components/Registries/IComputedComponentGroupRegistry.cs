using System;

namespace EcsR3.Computeds.Components.Registries
{
    public interface IComputedComponentGroupRegistry : IDisposable
    {
        IComputedComponentGroup<T1> GetComputedGroup<T1>();
        IComputedComponentGroup<T1, T2> GetComputedGroup<T1, T2>();
        IComputedComponentGroup<T1, T2, T3> GetComputedGroup<T1, T2, T3>();
        IComputedComponentGroup<T1, T2, T3, T4> GetComputedGroup<T1, T2, T3, T4>();
        IComputedComponentGroup<T1, T2, T3, T4, T5> GetComputedGroup<T1, T2, T3, T4, T5>();
    }
}