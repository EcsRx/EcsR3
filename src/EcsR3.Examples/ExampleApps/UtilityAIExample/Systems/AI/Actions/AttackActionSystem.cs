using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Components;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Types;
using EcsR3.Plugins.UtilityAI.Components;
using EcsR3.Plugins.UtilityAI.Systems;
using SystemsR3.Threading;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Systems.AI.Actions;

public class AttackActionSystem : AdviceActionSystem<CharacterDataComponent, CharacterActionComponent>
{
    public override int AdviceId => AdviceTypes.ShouldAttack;
    
    public AttackActionSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
    {}

    protected override void ActionAdvice(Entity entity, AgentComponent agentComponent, CharacterDataComponent characterDataComponent, CharacterActionComponent characterActionComponent)
    {
        characterActionComponent.CurrentAction = $"[red]Attacks the darkness doing {characterDataComponent.DamagePower} points of damage[/]";
    }
}