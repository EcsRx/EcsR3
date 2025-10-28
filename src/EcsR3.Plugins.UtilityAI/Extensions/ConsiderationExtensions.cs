using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Plugins.UtilityAI.Keys;
using EcsR3.Plugins.UtilityAI.Variables;

namespace EcsR3.Plugins.UtilityAI.Extensions
{
    public static class ConsiderationExtensions
    {
        public static float CalculateScore(this ReadOnlySpan<float> considerationScores)
        {
            var score = 1.0f;
            var compensationFactor = (float)(1.0 - 1.0 / considerationScores.Length);
            foreach (var utility in considerationScores)
            {
                var modification = (float)((1.0 - utility) * compensationFactor);
                var scaledUtility = utility + (modification * utility);
                score *= scaledUtility;
            }
            return score;
        }
        
        public static float CalculateScore(this float[] utilityScores)
        { return CalculateScore(new ReadOnlySpan<float>(utilityScores)); }
        
        public static float SumUtilityScore(this IConsiderationVariables variables, int considerationId)
        {
            var relatedUtilityKeys = variables.GetRelatedConsiderations(considerationId);
            var totalScore = 0.0f;
            for(var i=0; i<relatedUtilityKeys.Length; i++)
            { totalScore += relatedUtilityKeys[i].Score; }
            return totalScore;
        }

        public static float AverageUtilityScore(this IConsiderationVariables variables, int considerationId)
        {
            var relatedUtilityKeys = variables.GetRelatedConsiderations(considerationId);
            var totalScore = 0.0f;
            for(var i=0; i<relatedUtilityKeys.Length; i++)
            { totalScore += relatedUtilityKeys[i].Score; }
            return totalScore/relatedUtilityKeys.Length;
        }
        
        public static float MaxUtilityScore(this IConsiderationVariables variables, int considerationId)
        {
            var relatedUtilityKeys = variables.GetRelatedConsiderations(considerationId);
            var maxScore = 0.0f;
            for (var i = 0; i < relatedUtilityKeys.Length; i++)
            {
                var newScore = relatedUtilityKeys[i].Score;
                if(maxScore < newScore) { maxScore = newScore; }
            }
            return maxScore;
        }
        
        public static float CalculateScore(this IConsiderationVariables variables, int considerationId)
        { return variables.GetUtilityScores(considerationId).CalculateScore(); }
        
        public static float[] GetUtilityScores(this IConsiderationVariables variables, int considerationId)
        {
            var relatedConsiderationKeys = variables.GetRelatedConsiderations(considerationId);
            var scores = new float[relatedConsiderationKeys.Length];
            for(var i=0; i<relatedConsiderationKeys.Length; i++)
            { scores[i] = relatedConsiderationKeys[i].Score; }
            return scores;
        }
        
        public static IEnumerable<ConsiderationKeyWithScore> GetRankedUtility(this IConsiderationVariables variables, int considerationId)
        {
            return variables.GetRelatedConsiderations(considerationId)
                .OrderByDescending(x => x.Score);
        }
        
        public static ConsiderationKeyWithScore GetHighestScoredConsideration(this IConsiderationVariables variables, int considerationId)
        { return variables.GetRankedUtility(considerationId).First(); }
        
        public static ConsiderationKeyWithScore GetLowestScoredConsideration(this IConsiderationVariables variables, int considerationId)
        { return variables.GetRankedUtility(considerationId).Last(); }

        public static IEnumerable<(int RelatedId, float Score)> GetScoredRelatedIds(this IConsiderationVariables variables, int[] considerationIds)
        {
            var results = new Dictionary<int, float[]>();
            for (var i = 0; i < considerationIds.Length; i++)
            {
                var considerationId = considerationIds[i];
                var relatedUtilities = variables.GetRelatedConsiderations(considerationId);
                for (var j = 0; j < relatedUtilities.Length; j++)
                {
                    var relatedUtility = relatedUtilities[j];
                    var relatedId = relatedUtility.ConsiderationKey.RelatedId;
                    
                    if(!results.ContainsKey(relatedId))
                    { results.Add(relatedId, new float[considerationIds.Length]); }
                    
                    results[relatedId][i] = relatedUtility.Score;
                }
            }

            foreach (var result in results)
            { yield return new ValueTuple<int, float>(result.Key, result.Value.CalculateScore()); }
        }
        
        public static IEnumerable<KeyValuePair<ConsiderationKey, float>> GetLocalConsiderations(this IConsiderationVariables variables)
        { return variables.Where(x => x.Key.RelatedId == ConsiderationKey.NoRelatedIds); }

        public static IEnumerable<KeyValuePair<ConsiderationKey, float>> GetRelatedConsiderations(this IConsiderationVariables variables)
        { return variables.Where(x => x.Key.RelatedId != ConsiderationKey.NoRelatedIds); }
        
        public static ConsiderationKeyWithScore WithScore(this ConsiderationKey considerationKey, float score)
        { return new ConsiderationKeyWithScore(considerationKey, score); }

        
    }
}