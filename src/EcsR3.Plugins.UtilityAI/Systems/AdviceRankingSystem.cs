using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Plugins.UtilityAI.Components;
using EcsR3.Systems.Batching.Convention;
using R3;
using SystemsR3.Threading;

namespace EcsR3.Plugins.UtilityAI.Systems
{
    public class AdviceRankingSystem : BatchedSystem<AgentComponent>
    {
        public AdviceRankingSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<Unit> ReactWhen()
        { return Observable.EveryUpdate(); }

        protected override void Process(Entity entity, AgentComponent agentComponent)
        { agentComponent.AdviceVariables.RefreshAdviceRanking(); }
    }
}