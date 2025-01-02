using EcsR3.Components;

namespace EcsR3.Examples.ExampleApps.Performance.Components
{
    public class SimpleReadComponent : IComponent
    {
        public float StartingValue { get; set; } = 100.0f;
    }
}