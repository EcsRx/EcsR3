using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Components;
using EcsR3.Plugins.UtilityAI.Components;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Blueprints;

public class CharacterAgentBlueprint : IBlueprint
{
    public string Name { get; }
    public int MaxHealth { get; }
    public float DamagePower { get; }
    public float HealPower { get; }

    public CharacterAgentBlueprint(string name, int maxHealth,  float damagePower, float healPower)
    {
        Name = name;
        MaxHealth = maxHealth;
        DamagePower = damagePower;
        HealPower = healPower;
    }

    public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
    {
        entityComponentAccessor.AddComponents(entity, [new AgentComponent(), new CharacterDataComponent()
        {
            Name = Name,
            MaxHealth = MaxHealth,
            Health = MaxHealth,
            DamagePower = DamagePower,
            HealPower = HealPower
        }]);
    }
}