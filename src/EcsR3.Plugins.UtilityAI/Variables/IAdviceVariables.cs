using System.Collections.Generic;
using EcsR3.Plugins.UtilityAI.Keys;
using OpenRpg.Core.Variables;
using R3;

namespace EcsR3.Plugins.UtilityAI.Variables
{
    public interface IAdviceVariables : IVariables<float>
    {
        Observable<int> OnBestAdviceChanged { get; }
        
        int GetBestAdviceId();
        IEnumerable<AdviceWithScore> GetRankedAdvice();
        
        void RefreshAdviceRanking();
    }
}