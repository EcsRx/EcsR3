using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Components;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Types;
using EcsR3.Extensions;
using EcsR3.Plugins.UtilityAI.Components;
using EcsR3.Plugins.UtilityAI.Keys;
using EcsR3.Plugins.UtilityAI.Systems;
using SystemsR3.Threading;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Systems.AI.Advice;

public class ShouldHealSelfAdvice : AdviceSystem
{
    public static readonly ConsiderationKey HealingConsiderationKey = new (ConsiderationTypes.Healing);
    
    public override int AdviceId => AdviceTypes.ShouldHeal;

    protected override bool ShouldApplyTo(Entity entity, AgentComponent agentComponent)
    {
        var characterData = EntityComponentAccessor.GetComponent<CharacterDataComponent>(entity);
        return characterData.HealPower > 0;
    }

    public override ConsiderationLookup[] ConsiderationLookups { get; } = [new (ConsiderationTypes.Healing), new(ConsiderationTypes.LowHealth)];
    
    public ShouldHealSelfAdvice(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
    {}
}