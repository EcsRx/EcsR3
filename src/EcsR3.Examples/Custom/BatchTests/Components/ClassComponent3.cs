using EcsR3.Components;

namespace EcsR3.Examples.Custom.BatchTests.Components;

public class ClassComponent3 : IComponent
{
    public bool IsTrue { get; set; }
    public float Value { get; set; }
}