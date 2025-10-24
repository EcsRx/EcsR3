using EcsR3.Components;

namespace EcsR3.Examples.Custom.BatchTests.Components;

public struct StructComponent3Multiplexed : IComponent
{
    public bool IsTrue;
    public float Value;
}