using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Plugins.UtilityAI.Keys;
using EcsR3.Plugins.UtilityAI.Types;
using OpenRpg.Core.Variables;
using R3;

namespace EcsR3.Plugins.UtilityAI.Variables
{
    public class AdviceVariables : Variables<float>, IAdviceVariables
    {
        public AdviceVariables(IDictionary<int, float> internalVariables = null) : base(AdviceEngineVariableTypes.AdviceVariables, internalVariables)
        {}

        private readonly Subject<int> _onBestAdviceChanged = new Subject<int>();
        public Observable<int> OnBestAdviceChanged => _onBestAdviceChanged;
        
        public int[] SortedAdviceId { get; private set; } = Array.Empty<int>();

        public int GetBestAdviceId()
        { return SortedAdviceId.Length == 0 ? 0 : SortedAdviceId[0]; }

        public IEnumerable<AdviceWithScore> GetRankedAdvice()
        { return SortedAdviceId.Select(x => new AdviceWithScore(x, InternalVariables[x])); }

        public void RefreshAdviceRanking()
        {
            var startingAdviceId = GetBestAdviceId();
            
            SortedAdviceId = InternalVariables
                .OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .ToArray();

            var endingAdviceId = GetBestAdviceId();
            if(startingAdviceId != endingAdviceId)
            { _onBestAdviceChanged.OnNext(endingAdviceId); }
        }
    }
}