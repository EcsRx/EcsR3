using System;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Plugins.UtilityAI.Components;
using EcsR3.Plugins.UtilityAI.Extensions;
using EcsR3.Plugins.UtilityAI.Keys;
using EcsR3.Plugins.UtilityAI.Types;
using EcsR3.Systems.Batching.Convention;
using EcsR3.Systems.Reactive;
using R3;
using SystemsR3.Threading;

namespace EcsR3.Plugins.UtilityAI.Systems
{
    public abstract class AdviceSystem : BatchedSystem<AgentComponent>, ISetupSystem
    {
        /// <summary>
        /// The type/id of the advice
        /// </summary>
        public abstract int AdviceId { get; }
        
        /// <summary>
        /// The category this advice falls under
        /// </summary>
        /// <remarks>aka bucket types, it allows you to group certain action types</remarks>
        public virtual int CategoryId => AdviceCategoryType.Default;
        
        /// <summary>
        /// The considerations that should be factored in to score the advice
        /// </summary>
        public abstract ConsiderationLookup[] ConsiderationLookups { get; }

        protected AdviceSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}
        
        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }
        
        /// <summary>
        /// Allows you to provide custom modifiers, be it a static value or a dynamic expression
        /// </summary>
        /// <param name="score">The calculated score</param>
        /// <param name="entity">The associated entity</param>
        /// <param name="agentComponent">The associated agent</param>
        /// <returns>The modified/weighted score value</returns>
        protected virtual float ModifyAdviceScore(float score, Entity entity, AgentComponent agentComponent)
        { return score; }
        
        /// <summary>
        /// A predicate to decide if an entity should calculate/support this advice
        /// </summary>
        /// <param name="entity">The associated entity</param>
        /// <param name="agentComponent">The associated agent</param>
        /// <returns>True if the agent supports this advice, False if it does not</returns>
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

        /// <summary>
        /// Internally sets up the entity verifying if it should have this advice applied or not
        /// </summary>
        /// <param name="entityComponentAccessor">The entity component accessor</param>
        /// <param name="entity">The associated entity</param>
        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var agent = entityComponentAccessor.GetComponent<AgentComponent>(entity);
            if(!ShouldApplyTo(entity, agent)) { return; }
            agent.ActiveAdvice.Add(AdviceId);
        }
    }
}