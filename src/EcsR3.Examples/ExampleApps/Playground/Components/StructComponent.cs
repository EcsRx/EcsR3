using System.Numerics;
using System.Runtime.InteropServices;
using EcsR3.Components;

namespace EcsR3.Examples.ExampleApps.Playground.Components
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StructComponent : IComponent
    {
        public Vector3 Position { get; set; }
        public float Something { get; set; }
    }
}