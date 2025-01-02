using System;
using EcsR3.Components;

namespace EcsR3.Tests.Models
{
    public class TestDisposableComponent : IComponent, IDisposable
    {
        public bool isDisposed = false;

        public void Dispose()
        { isDisposed = true; }
    }
}