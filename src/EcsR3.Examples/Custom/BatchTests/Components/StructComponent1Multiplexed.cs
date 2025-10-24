using System.Numerics;
using EcsR3.Components;

namespace EcsR3.Examples.Custom.BatchTests.Components;

public struct StructComponent1Multiplexed : IComponent
{
    public Vector3 Position;
    public float Something;
}