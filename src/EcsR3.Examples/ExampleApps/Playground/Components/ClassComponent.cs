using System.Numerics;
using EcsR3.Components;

namespace EcsR3.Examples.ExampleApps.Playground.Components
{
    public class ClassComponent : IComponent
    {
        public Vector3 Position { get; set; }
        public float Something { get; set; }
    }
}