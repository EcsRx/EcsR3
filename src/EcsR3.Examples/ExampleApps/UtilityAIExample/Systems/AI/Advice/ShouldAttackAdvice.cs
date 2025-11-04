using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Types;
using EcsR3.Plugins.UtilityAI.Components;
using EcsR3.Plugins.UtilityAI.Keys;
using EcsR3.Plugins.UtilityAI.Systems;
using SystemsR3.Threading;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Systems.AI.Advice;

public class ShouldAttackAdvice : AdviceSystem
{
    public override int AdviceId => AdviceTypes.ShouldAttack;

    public override ConsiderationLookup[] ConsiderationLookups { get; } = [new(ConsiderationTypes.Power), new(ConsiderationTypes.Health)];
    
    public ShouldAttackAdvice(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
    {}
}