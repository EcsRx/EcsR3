using EcsR3.Plugins.UtilityAI.Systems;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;

namespace EcsR3.Plugins.UtilityAI.Modules
{
    public class UtilityAIModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            registry.BindSystem<AdviceRankingSystem>();
        }
    }
}