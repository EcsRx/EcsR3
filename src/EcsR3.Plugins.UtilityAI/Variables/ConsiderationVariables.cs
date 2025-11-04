using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Plugins.UtilityAI.Keys;
using EcsR3.Plugins.UtilityAI.Types;
using OpenRpg.Core.Variables;

namespace EcsR3.Plugins.UtilityAI.Variables
{
    public class ConsiderationVariables : KeyedVariables<ConsiderationKey, float>, IConsiderationVariables
    {
        public ConsiderationVariables(IDictionary<ConsiderationKey, float> internalVariables = null) : base(AdviceEngineVariableTypes.UtilityVariables, internalVariables)
        {}

        public ConsiderationKeyWithScore[] GetRelatedConsiderations(int considerationId)
        {
            Span<ConsiderationKeyWithScore> pairs = stackalloc ConsiderationKeyWithScore[Count];
            var indexesUsed = 0;

            foreach (var variable in InternalVariables)
            {
                if(variable.Key.ConsiderationId != considerationId) { continue; }
                pairs[indexesUsed++] = new ConsiderationKeyWithScore(variable.Key, variable.Value);
            }
            
            return pairs[..indexesUsed].ToArray();
        }

        public void Remove(int considerationId)
        {
            var relatedUtilityKeys = InternalVariables.Keys.Where(x => x.ConsiderationId == considerationId).ToArray();
            foreach(var relatedKey in relatedUtilityKeys)
            { InternalVariables.Remove(relatedKey); }
        }

        public bool Contains(int considerationId)
        {
            return InternalVariables
                .Any(variable => variable.Key.ConsiderationId == considerationId);
        }
        
        public float this[int considerationId]
        {
            get => InternalVariables[new ConsiderationKey(considerationId)];
            set => this[new ConsiderationKey(considerationId)] = value;
        }
    }
}