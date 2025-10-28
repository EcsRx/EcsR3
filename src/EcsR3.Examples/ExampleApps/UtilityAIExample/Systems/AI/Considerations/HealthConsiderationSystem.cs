using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Components;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Types;
using EcsR3.Plugins.UtilityAI.Clampers;
using EcsR3.Plugins.UtilityAI.Keys;
using EcsR3.Plugins.UtilityAI.Systems;
using EcsR3.Plugins.UtilityAI.Variables;
using OpenRpg.CurveFunctions;
using SystemsR3.Threading;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Systems.AI.Considerations;

public class HealthConsiderationSystem : ConsiderationSystem<CharacterDataComponent>
{
    public override int ConsiderationId => ConsiderationTypes.Health;
    public override ICurveFunction Evaluator => PresetCurves.Linear;
    public override IClamper Clamper => PresetClampers.ZeroToOne;
    
    public HealthConsiderationSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
    {}

    protected override ConsiderationKeyWithScore[] CalculateUtility(Entity entity, IConsiderationVariables considerationVariables, CharacterDataComponent characterComponent)
    {
        var utilityKey = new ConsiderationKey(ConsiderationId);
        var healthPercentage = GetPercentageFor(characterComponent.MaxHealth, characterComponent.Health);
        var score = CalculateScore(healthPercentage, entity, characterComponent);
        return new[] { new ConsiderationKeyWithScore(utilityKey, score) };
    }

    public float GetPercentageFor(float max, float current)
    { return current / max; }
}