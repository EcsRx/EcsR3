using System.Collections.Generic;
using EcsR3.Plugins.UtilityAI.Keys;
using OpenRpg.Core.Variables;

namespace EcsR3.Plugins.UtilityAI.Variables
{
    public interface IConsiderationVariables : IKeyedVariables<ConsiderationKey, float>
    {
        ConsiderationKeyWithScore[] GetRelatedConsiderations(int considerationId);
        void Remove(int considerationId);
        bool Contains(int considerationId);

        float this[int considerationId] { get; set; }
    }
}