using EcsR3.Components;

namespace EcsR3.Examples.ExampleApps.ComputedGroupExample.Components
{
    public class HasHealthComponent : IComponent
    {
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
    }
}