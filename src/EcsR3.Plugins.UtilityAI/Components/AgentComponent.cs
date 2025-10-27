using System.Collections.Generic;
using EcsR3.Components;
using EcsR3.Plugins.UtilityAI.Variables;

namespace EcsR3.Plugins.UtilityAI.Components
{
    public class AgentComponent : IComponent
    {
        public IConsiderationVariables ConsiderationVariables { get; } = new ConsiderationVariables();
        public IAdviceVariables AdviceVariables { get; } = new AdviceVariables();
        
        public HashSet<int> ActiveConsiderations { get; } = new HashSet<int>();
        public HashSet<int> ActiveAdvice { get; } = new HashSet<int>();
    }
}