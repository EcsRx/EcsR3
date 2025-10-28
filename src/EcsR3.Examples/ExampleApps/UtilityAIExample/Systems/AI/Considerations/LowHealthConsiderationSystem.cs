using System.Collections.Generic;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Types;
using EcsR3.Plugins.UtilityAI.Clampers;
using EcsR3.Plugins.UtilityAI.Keys;
using EcsR3.Plugins.UtilityAI.Systems;
using EcsR3.Plugins.UtilityAI.Variables;
using OpenRpg.CurveFunctions;
using SystemsR3.Threading;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Systems.AI.Considerations;

public class LowHealthConsiderationSystem : ConsiderationSystem
{
    public static readonly ConsiderationKey HealthUtilityKey = new(ConsiderationTypes.Health);
    
    public override int ConsiderationId => ConsiderationTypes.LowHealth;
    public override ICurveFunction Evaluator => PresetCurves.InverseLinear;
    public override IClamper Clamper => PresetClampers.ZeroToOne;
    
    public LowHealthConsiderationSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
    {}

    protected override ConsiderationKeyWithScore[] CalculateUtility(Entity entity, IConsiderationVariables considerationVariables)
    {
        var considerationKey = new ConsiderationKey(ConsiderationId);
        var healthConsiderationScore = considerationVariables.GetValueOrDefault(HealthUtilityKey, 0);
        var score = CalculateScore(healthConsiderationScore, entity);
        return new[] { new ConsiderationKeyWithScore(considerationKey, score) };
    }
}