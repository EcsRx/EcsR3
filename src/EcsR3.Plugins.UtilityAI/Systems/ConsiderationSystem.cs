using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Plugins.UtilityAI.Clampers;
using EcsR3.Plugins.UtilityAI.Components;
using EcsR3.Plugins.UtilityAI.Keys;
using EcsR3.Plugins.UtilityAI.Variables;
using EcsR3.Systems.Batching.Convention;
using EcsR3.Systems.Reactive;
using OpenRpg.CurveFunctions;
using OpenRpg.CurveFunctions.Extensions;
using R3;
using SystemsR3.Threading;

namespace EcsR3.Plugins.UtilityAI.Systems
{
    public abstract class ConsiderationSystem : BatchedSystem<AgentComponent>, ISetupSystem
    {
        public abstract int ConsiderationId { get; }
        public abstract ICurveFunction Evaluator { get; }
        public abstract IClamper Clamper { get; }
        
        public ConsiderationSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected virtual float CalculateScore(float inputValue, Entity entity)
        {
            var clampedValue = Clamper.Clamp(inputValue);
            var rawScore = Evaluator.Plot(clampedValue);
            var modifiedScore = ModifyScore(rawScore, entity);
            return modifiedScore.SanitizeAndClamp();
        }

        protected virtual float ModifyScore(float score, Entity entity)
        { return score; }
        
        protected virtual bool ShouldApplyTo(Entity entity, AgentComponent agentComponent) => true;
        
        protected override void Process(Entity entity, AgentComponent agentComponent)
        {
            if(!agentComponent.ActiveConsiderations.Contains(ConsiderationId)){ return; }
            
            var updatedUtilities = CalculateUtility(entity, agentComponent.ConsiderationVariables);
            for (var i = 0; i < updatedUtilities.Length; i++)
            {
                var updatedUtility = updatedUtilities[i];
                agentComponent.ConsiderationVariables[updatedUtility.ConsiderationKey] = updatedUtility.Score;
            }
        }
        
        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var agent = entityComponentAccessor.GetComponent<AgentComponent>(entity);
            if(!ShouldApplyTo(entity, agent)) { return; }
            agent.ActiveConsiderations.Add(ConsiderationId);
        }
        
        protected abstract ConsiderationKeyWithScore[] CalculateUtility(Entity entity, IConsiderationVariables considerationVariables);
    }
    
    public abstract class ConsiderationSystem<T1> : BatchedSystem<AgentComponent, T1>, ISetupSystem
        where T1 : IComponent
    {
        public abstract int ConsiderationId { get; }
        public abstract ICurveFunction Evaluator { get; }
        public abstract IClamper Clamper { get; }
        
        public ConsiderationSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected virtual float CalculateScore(float inputValue, Entity entity, T1 component)
        {
            var clampedValue = Clamper.Clamp(inputValue);
            var rawScore = Evaluator.Plot(clampedValue);
            var modifiedScore = ModifyScore(rawScore, entity, component);
            return modifiedScore.SanitizeAndClamp();
        }

        protected virtual float ModifyScore(float score, Entity entity, T1 component)
        { return score; }
        
        protected virtual bool ShouldApplyTo(Entity entity, AgentComponent agentComponent) => true;
        
        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1)
        {
            if(!agentComponent.ActiveConsiderations.Contains(ConsiderationId)){ return; }
            
            var updatedUtilities = CalculateUtility(entity, agentComponent.ConsiderationVariables, component1);
            for (var i = 0; i < updatedUtilities.Length; i++)
            {
                var updatedUtility = updatedUtilities[i];
                agentComponent.ConsiderationVariables[updatedUtility.ConsiderationKey] = updatedUtility.Score;
            }
        }
        
        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var agent = entityComponentAccessor.GetComponent<AgentComponent>(entity);
            if(!ShouldApplyTo(entity, agent)) { return; }
            agent.ActiveConsiderations.Add(ConsiderationId);
        }
        
        protected abstract ConsiderationKeyWithScore[] CalculateUtility(Entity entity, IConsiderationVariables considerationVariables, T1 component1);
    }
    
    public abstract class ConsiderationSystem<T1, T2> : BatchedSystem<AgentComponent, T1, T2>, ISetupSystem
        where T1 : IComponent
        where T2 : IComponent
    {
        public abstract int ConsiderationId { get; }
        public abstract ICurveFunction Evaluator { get; }
        public abstract IClamper Clamper { get; }
        
        public ConsiderationSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected virtual float CalculateScore(float inputValue, Entity entity, T1 component1, T2 component2)
        {
            var clampedValue = Clamper.Clamp(inputValue);
            var rawScore = Evaluator.Plot(clampedValue);
            var modifiedScore = ModifyScore(rawScore, entity, component1, component2);
            return modifiedScore.SanitizeAndClamp();
        }

        protected virtual float ModifyScore(float score, Entity entity, T1 component1, T2 component2)
        { return score; }
        
        protected virtual bool ShouldApplyTo(Entity entity, AgentComponent agentComponent) => true;
        
        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1, T2 component2)
        {
            if(!agentComponent.ActiveConsiderations.Contains(ConsiderationId)){ return; }
            
            var updatedUtilities = CalculateUtility(entity, agentComponent.ConsiderationVariables, component1, component2);
            for (var i = 0; i < updatedUtilities.Length; i++)
            {
                var updatedUtility = updatedUtilities[i];
                agentComponent.ConsiderationVariables[updatedUtility.ConsiderationKey] = updatedUtility.Score;
            }
        }
        
        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var agent = entityComponentAccessor.GetComponent<AgentComponent>(entity);
            if(!ShouldApplyTo(entity, agent)) { return; }
            agent.ActiveConsiderations.Add(ConsiderationId);
        }
        
        protected abstract ConsiderationKeyWithScore[] CalculateUtility(Entity entity, IConsiderationVariables considerationVariables, T1 component1, T2 component2);
    }
    
    public abstract class ConsiderationSystem<T1, T2, T3> : BatchedSystem<AgentComponent, T1, T2, T3>, ISetupSystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        public abstract int ConsiderationId { get; }
        public abstract ICurveFunction Evaluator { get; }
        public abstract IClamper Clamper { get; }
        
        public ConsiderationSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected virtual float CalculateScore(float inputValue, Entity entity, T1 component1, T2 component2, T3 component3)
        {
            var clampedValue = Clamper.Clamp(inputValue);
            var rawScore = Evaluator.Plot(clampedValue);
            var modifiedScore = ModifyScore(rawScore, entity, component1, component2, component3);
            return modifiedScore.SanitizeAndClamp();
        }

        protected virtual float ModifyScore(float score, Entity entity, T1 component1, T2 component2, T3 component3)
        { return score; }
        
        protected virtual bool ShouldApplyTo(Entity entity, AgentComponent agentComponent) => true;
        
        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1, T2 component2, T3 component3)
        {
            if(!agentComponent.ActiveConsiderations.Contains(ConsiderationId)){ return; }
            
            var updatedUtilities = CalculateUtility(entity, agentComponent.ConsiderationVariables, component1, component2, component3);
            for (var i = 0; i < updatedUtilities.Length; i++)
            {
                var updatedUtility = updatedUtilities[i];
                agentComponent.ConsiderationVariables[updatedUtility.ConsiderationKey] = updatedUtility.Score;
            }
        }
        
        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var agent = entityComponentAccessor.GetComponent<AgentComponent>(entity);
            if(!ShouldApplyTo(entity, agent)) { return; }
            agent.ActiveConsiderations.Add(ConsiderationId);
        }
        
        protected abstract ConsiderationKeyWithScore[] CalculateUtility(Entity entity, IConsiderationVariables considerationVariables, T1 component1, T2 component2, T3 component3);
    }
    
    public abstract class ConsiderationSystem<T1, T2, T3, T4> : BatchedSystem<AgentComponent, T1, T2, T3, T4>, ISetupSystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
    {
        public abstract int ConsiderationId { get; }
        public abstract ICurveFunction Evaluator { get; }
        public abstract IClamper Clamper { get; }
        
        public ConsiderationSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected virtual float CalculateScore(float inputValue, Entity entity, T1 component1, T2 component2, T3 component3, T4 component4)
        {
            var clampedValue = Clamper.Clamp(inputValue);
            var rawScore = Evaluator.Plot(clampedValue);
            var modifiedScore = ModifyScore(rawScore, entity, component1, component2, component3, component4);
            return modifiedScore.SanitizeAndClamp();
        }

        protected virtual float ModifyScore(float score, Entity entity, T1 component1, T2 component2, T3 component3, T4 component4)
        { return score; }
        
        protected virtual bool ShouldApplyTo(Entity entity, AgentComponent agentComponent) => true;
        
        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1, T2 component2, T3 component3, T4 component4)
        {
            if(!agentComponent.ActiveConsiderations.Contains(ConsiderationId)){ return; }
            
            var updatedUtilities = CalculateUtility(entity, agentComponent.ConsiderationVariables, component1, component2, component3, component4);
            for (var i = 0; i < updatedUtilities.Length; i++)
            {
                var updatedUtility = updatedUtilities[i];
                agentComponent.ConsiderationVariables[updatedUtility.ConsiderationKey] = updatedUtility.Score;
            }
        }
        
        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var agent = entityComponentAccessor.GetComponent<AgentComponent>(entity);
            if(!ShouldApplyTo(entity, agent)) { return; }
            agent.ActiveConsiderations.Add(ConsiderationId);
        }
        
        protected abstract ConsiderationKeyWithScore[] CalculateUtility(Entity entity, IConsiderationVariables considerationVariables, T1 component1, T2 component2, T3 component3, T4 component4);
    }
    
    public abstract class ConsiderationSystem<T1, T2, T3, T4, T5> : BatchedSystem<AgentComponent, T1, T2, T3, T4, T5>, ISetupSystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
    {
        public abstract int ConsiderationId { get; }
        public abstract ICurveFunction Evaluator { get; }
        public abstract IClamper Clamper { get; }
        
        public ConsiderationSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected virtual float CalculateUtilityScore(float inputValue, Entity entity, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5)
        {
            var clampedValue = Clamper.Clamp(inputValue);
            var rawScore = Evaluator.Plot(clampedValue);
            var modifiedScore = ModifyUtilityScore(rawScore, entity, component1, component2, component3, component4, component5);
            return modifiedScore.SanitizeAndClamp();
        }

        protected virtual float ModifyUtilityScore(float score, Entity entity, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5)
        { return score; }
        
        protected virtual bool ShouldApplyTo(Entity entity, AgentComponent agentComponent) => true;
        
        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5)
        {
            if(!agentComponent.ActiveConsiderations.Contains(ConsiderationId)){ return; }
            
            var updatedUtilities = CalculateUtility(entity, agentComponent.ConsiderationVariables, component1, component2, component3, component4, component5);
            for (var i = 0; i < updatedUtilities.Length; i++)
            {
                var updatedUtility = updatedUtilities[i];
                agentComponent.ConsiderationVariables[updatedUtility.ConsiderationKey] = updatedUtility.Score;
            }
        }
        
        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var agent = entityComponentAccessor.GetComponent<AgentComponent>(entity);
            if(!ShouldApplyTo(entity, agent)) { return; }
            agent.ActiveConsiderations.Add(ConsiderationId);
        }
        
        protected abstract ConsiderationKeyWithScore[] CalculateUtility(Entity entity, IConsiderationVariables considerationVariables, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5);
    }
    
    public abstract class ConsiderationSystem<T1, T2, T3, T4, T5, T6> : BatchedSystem<AgentComponent, T1, T2, T3, T4, T5, T6>, ISetupSystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
    {
        public abstract int ConsiderationId { get; }
        public abstract ICurveFunction Evaluator { get; }
        public abstract IClamper Clamper { get; }
        
        public ConsiderationSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected virtual float CalculateScore(float inputValue, Entity entity, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6)
        {
            var clampedValue = Clamper.Clamp(inputValue);
            var rawScore = Evaluator.Plot(clampedValue);
            var modifiedScore = ModifyScore(rawScore, entity, component1, component2, component3, component4, component5, component6);
            return modifiedScore.SanitizeAndClamp();
        }

        protected virtual float ModifyScore(float score, Entity entity, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6)
        { return score; }
        
        protected virtual bool ShouldApplyTo(Entity entity, AgentComponent agentComponent) => true;
        
        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6)
        {
            if(!agentComponent.ActiveConsiderations.Contains(ConsiderationId)){ return; }
            
            var updatedUtilities = CalculateUtility(entity, agentComponent.ConsiderationVariables, component1, component2, component3, component4, component5, component6);
            for (var i = 0; i < updatedUtilities.Length; i++)
            {
                var updatedUtility = updatedUtilities[i];
                agentComponent.ConsiderationVariables[updatedUtility.ConsiderationKey] = updatedUtility.Score;
            }
        }
        
        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var agent = entityComponentAccessor.GetComponent<AgentComponent>(entity);
            if(!ShouldApplyTo(entity, agent)) { return; }
            agent.ActiveConsiderations.Add(ConsiderationId);
        }
        
        protected abstract ConsiderationKeyWithScore[] CalculateUtility(Entity entity, IConsiderationVariables considerationVariables, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6);
    }
}