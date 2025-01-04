using System;
using EcsR3.Components;
using R3;

namespace EcsR3.Tests.Models
{
    public class ComponentWithReactiveProperty : IComponent, IDisposable
    {
        public ReactiveProperty<int> SomeNumber { get; } = new ReactiveProperty<int>();

        public void Dispose()
        { SomeNumber?.Dispose(); }
    }
}