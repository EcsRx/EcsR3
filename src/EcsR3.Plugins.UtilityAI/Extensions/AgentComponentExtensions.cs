using System.Collections.Generic;
using EcsR3.Plugins.UtilityAI.Components;
using EcsR3.Plugins.UtilityAI.Keys;

namespace EcsR3.Plugins.UtilityAI.Extensions
{
    public static class AgentComponentExtensions
    {
        public static int GetBestAdviceId(this AgentComponent agent)
        { return agent.AdviceVariables.GetBestAdviceId(); }
        
        public static IEnumerable<AdviceWithScore> GetRankedAdvice(this AgentComponent agent)
        { return agent.AdviceVariables.GetRankedAdvice(); }
    }
}