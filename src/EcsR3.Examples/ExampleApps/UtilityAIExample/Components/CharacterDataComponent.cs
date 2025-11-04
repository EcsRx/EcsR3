using System.Numerics;
using EcsR3.Components;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Components;

public class CharacterDataComponent : IComponent
{
    public string Name { get; set; }
    public int MaxHealth { get; set; }
    public int Health { get; set; }
    public float DamagePower { get; set; }
    public float HealPower { get; set; }
}