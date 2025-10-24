using EcsR3.Components;

namespace EcsR3.Examples.Custom.BatchTests.Components;

public struct StructComponent2Multiplexed : IComponent
{
    public bool IsTrue;
    public int Value;
}