using System;
using EcsR3.Components;
using R3;

namespace EcsR3.Examples.ExampleApps.HealthExample.Components
{
    public class HealthComponent : IComponent, IDisposable
    {
        public ReactiveProperty<float> Health { get; set; }
        public float MaxHealth { get; set; }
        
        public void Dispose()
        {
            Health.Dispose();
        }
    }
}