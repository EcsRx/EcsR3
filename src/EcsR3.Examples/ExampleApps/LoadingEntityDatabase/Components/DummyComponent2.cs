using System.Numerics;
using EcsR3.Components;

namespace EcsR3.Examples.ExampleApps.LoadingEntityDatabase.Components
{
    public class DummyComponent2 : IComponent
    {
        public Vector3 SomeVector { get; set; }
        public Quaternion SomeQuaternion { get; set; }
    }
}