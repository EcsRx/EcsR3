using OpenRpg.Core.Utils;

namespace EcsR3.Plugins.UtilityAI.Clampers
{
    public interface IClamper
    {
        RangeF Range { get; }
        bool Normalize { get; }

        float Clamp(float input);
    }
}