using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Plugins.UtilityAI.Components;
using EcsR3.Plugins.UtilityAI.Extensions;
using EcsR3.Systems.Batching.Convention;
using R3;
using SystemsR3.Threading;

namespace EcsR3.Plugins.UtilityAI.Systems
{
    public abstract class AdviceActionSystem : BatchedSystem<AgentComponent>
    {
        public abstract int AdviceId { get; }
        
        public AdviceActionSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected override void Process(Entity entity, AgentComponent agentComponent)
        {
            var bestAdviceId = agentComponent.GetBestAdviceId();
            if(bestAdviceId != AdviceId) { return; }
            
            ActionAdvice(entity, agentComponent);
        }
        
        protected abstract void ActionAdvice(Entity entity, AgentComponent agentComponent);
    }
    
    public abstract class AdviceActionSystem<T1> : BatchedSystem<AgentComponent, T1> 
        where T1 : IComponent
    {
        public abstract int AdviceId { get; }
        
        public AdviceActionSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1)
        {
            var bestAdviceId = agentComponent.GetBestAdviceId();
            if(bestAdviceId != AdviceId) { return; }
            
            ActionAdvice(entity, agentComponent, component1);
        }
        
        protected abstract void ActionAdvice(Entity entity, AgentComponent agentComponent, T1 component);
    }
        
    public abstract class AdviceActionSystem<T1, T2> : BatchedSystem<AgentComponent, T1, T2> 
        where T1 : IComponent where T2 : IComponent
    {
        public abstract int AdviceId { get; }
        
        public AdviceActionSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1, T2 component2)
        {
            var bestAdviceId = agentComponent.GetBestAdviceId();
            if(bestAdviceId != AdviceId) { return; }
            
            ActionAdvice(entity, agentComponent, component1, component2);
        }
        
        protected abstract void ActionAdvice(Entity entity, AgentComponent agentComponent, T1 component, T2 component2);
    }
    
    public abstract class AdviceActionSystem<T1, T2, T3> : BatchedSystem<AgentComponent, T1, T2, T3> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        public abstract int AdviceId { get; }
        
        public AdviceActionSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1, T2 component2, T3 component3)
        {
            var bestAdviceId = agentComponent.GetBestAdviceId();
            if(bestAdviceId != AdviceId) { return; }
            
            ActionAdvice(entity, agentComponent, component1, component2, component3);
        }
        
        protected abstract void ActionAdvice(Entity entity, AgentComponent agentComponent, T1 component, T2 component2, T3 component3);
    }
        
    public abstract class AdviceActionSystem<T1, T2, T3, T4> : BatchedSystem<AgentComponent, T1, T2, T3, T4> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
    {
        public abstract int AdviceId { get; }
        
        public AdviceActionSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1, T2 component2, T3 component3, T4 component4)
        {
            var bestAdviceId = agentComponent.GetBestAdviceId();
            if(bestAdviceId != AdviceId) { return; }
            
            ActionAdvice(entity, agentComponent, component1, component2, component3, component4);
        }
        
        protected abstract void ActionAdvice(Entity entity, AgentComponent agentComponent, T1 component, T2 component2, T3 component3, T4 component4);
    }
        
    public abstract class AdviceActionSystem<T1, T2, T3, T4, T5> : BatchedSystem<AgentComponent, T1, T2, T3, T4, T5> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent
    {
        public abstract int AdviceId { get; }
        
        public AdviceActionSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5)
        {
            var bestAdviceId = agentComponent.GetBestAdviceId();
            if(bestAdviceId != AdviceId) { return; }
            
            ActionAdvice(entity, agentComponent, component1, component2, component3, component4, component5);
        }
        
        protected abstract void ActionAdvice(Entity entity, AgentComponent agentComponent, T1 component, T2 component2, T3 component3, T4 component4, T5 component5);
    }
        
    public abstract class AdviceActionSystem<T1, T2, T3, T4, T5, T6> : BatchedSystem<AgentComponent, T1, T2, T3, T4, T5, T6> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent
    {
        public abstract int AdviceId { get; }
        
        public AdviceActionSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected override void Process(Entity entity, AgentComponent agentComponent, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6)
        {
            var bestAdviceId = agentComponent.GetBestAdviceId();
            if(bestAdviceId != AdviceId) { return; }
            
            ActionAdvice(entity, agentComponent, component1, component2, component3, component4, component5, component6);
        }
        
        protected abstract void ActionAdvice(Entity entity, AgentComponent agentComponent, T1 component, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6);
    }
}