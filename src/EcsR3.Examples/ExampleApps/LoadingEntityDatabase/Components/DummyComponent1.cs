using System;
using EcsR3.Components;

namespace EcsR3.Examples.ExampleApps.LoadingEntityDatabase.Components
{
    public class DummyComponent1 : IComponent
    {
        public int SomeNumber { get; set; }
        public string SomeString { get; set; }
        public DateTime SomeTime { get; set; }
    }
}