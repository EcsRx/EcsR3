using EcsR3.Components;

namespace EcsR3.Examples.Custom.BatchTests.Components
{
    public class ClassComponent2 : IComponent
    {
        public bool IsTrue { get; set; }
        public int Value { get; set; }
    }
}