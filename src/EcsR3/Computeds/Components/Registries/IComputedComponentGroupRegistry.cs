using System;
using EcsR3.Components;

namespace EcsR3.Computeds.Components.Registries
{
    public interface IComputedComponentGroupRegistry : IDisposable
    {
        IComputedComponentGroup<T1> GetComputedGroup<T1>() where T1 : IComponent;
        IComputedComponentGroup<T1, T2> GetComputedGroup<T1, T2>() where T1 : IComponent where T2 : IComponent;
        IComputedComponentGroup<T1, T2, T3> GetComputedGroup<T1, T2, T3>() where T1 : IComponent where T2 : IComponent where T3 : IComponent;
        IComputedComponentGroup<T1, T2, T3, T4> GetComputedGroup<T1, T2, T3, T4>() where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent;
        IComputedComponentGroup<T1, T2, T3, T4, T5> GetComputedGroup<T1, T2, T3, T4, T5>() where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent;
        IComputedComponentGroup<T1, T2, T3, T4, T5, T6> GetComputedGroup<T1, T2, T3, T4, T5, T6>() where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent;
        IComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7> GetComputedGroup<T1, T2, T3, T4, T5, T6, T7>() where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent where T7 : IComponent;
    }
}