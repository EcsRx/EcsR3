using System;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Plugins.UtilityAI.Components;
using EcsR3.Plugins.UtilityAI.Extensions;
using EcsR3.Plugins.UtilityAI.Keys;
using EcsR3.Systems.Batching.Convention;
using EcsR3.Systems.Reactive;
using R3;
using SystemsR3.Threading;

namespace EcsR3.Plugins.UtilityAI.Systems
{
    public abstract class AdviceSystem : BatchedSystem<AgentComponent>, ISetupSystem
    {
        public abstract int AdviceId { get; }
        public abstract ConsiderationLookup[] ConsiderationLookups { get; }

        protected AdviceSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}
        
        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }
        
        protected virtual float ModifyAdviceScore(float score, Entity entity, AgentComponent agentComponent)
        { return score; }
        
        protected virtual bool ShouldApplyTo(Entity entity, AgentComponent agentComponent) => true;
        
        protected override void Process(Entity entity, AgentComponent agentComponent)
        {
            if(!agentComponent.ActiveAdvice.Contains(AdviceId)) { return; }
            
            Span<float> utilityScores = stackalloc float[ConsiderationLookups.Length];
            var indexesUsed = 0;
            for (var i = 0; i < ConsiderationLookups.Length; i++)
            {
                var considerationLookup = ConsiderationLookups[i];
                
                if(!agentComponent.ActiveConsiderations.Contains(considerationLookup.ConsiderationId)) { continue; }

                if (!considerationLookup.HasRelatedData)
                { 
                    var considerationKey = new ConsiderationKey(considerationLookup.ConsiderationId);
                    if (agentComponent.ConsiderationVariables.ContainsKey(considerationKey))
                    { utilityScores[indexesUsed++] = agentComponent.ConsiderationVariables[considerationLookup.ConsiderationId]; }
                    else
                    { utilityScores[indexesUsed++] = 0; }
                    continue;
                }

                var highestScore = 0.0f;
                var relatedConsiderations = agentComponent.ConsiderationVariables.GetRelatedConsiderations(considerationLookup.ConsiderationId);
                for (var j = 0; j < relatedConsiderations.Length; j++)
                {
                    var score = relatedConsiderations[j].Score;
                    if(score > highestScore) { highestScore = score; }
                }
                utilityScores[indexesUsed++] = highestScore;
            }

            ReadOnlySpan<float> scoresToUse = utilityScores[..indexesUsed];
            var calculatedScore = scoresToUse.CalculateScore();
            var finalScore = ModifyAdviceScore(calculatedScore, entity, agentComponent);
            agentComponent.AdviceVariables[AdviceId] = finalScore;
        }

        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var agent = entityComponentAccessor.GetComponent<AgentComponent>(entity);
            if(!ShouldApplyTo(entity, agent)) { return; }
            agent.ActiveAdvice.Add(AdviceId);
        }
    }
}