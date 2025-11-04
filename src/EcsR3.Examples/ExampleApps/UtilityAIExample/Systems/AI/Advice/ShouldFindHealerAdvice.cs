using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Types;
using EcsR3.Plugins.UtilityAI.Keys;
using EcsR3.Plugins.UtilityAI.Systems;
using SystemsR3.Threading;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Systems.AI.Advice;

public class ShouldFindHealerAdvice : AdviceSystem
{
    public override int AdviceId => AdviceTypes.ShouldFindHealer;
    
    public override ConsiderationLookup[] ConsiderationLookups { get; } = [new(ConsiderationTypes.LowHealth)];
    
    public ShouldFindHealerAdvice(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
    {}
}