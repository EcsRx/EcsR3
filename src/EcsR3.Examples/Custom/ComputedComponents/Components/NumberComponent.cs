using EcsR3.Components;

namespace EcsR3.Examples.Custom.ComputedComponents.Components;

public class NumberComponent : IComponent
{
    public int Value;
}

public class Number2Component : IComponent
{
    public int Value;
}

public struct StructNumberComponent : IComponent
{
    public int Value;
}

public struct StructNumber2Component : IComponent
{
    public int Value;
}