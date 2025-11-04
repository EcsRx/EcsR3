using OpenRpg.Core.Utils;

namespace EcsR3.Plugins.UtilityAI.Clampers
{
    public class PassThroughClamper : IClamper
    {
        public RangeF Range { get;  }
        public bool Normalize { get; }
        
        public float Clamp(float input)
        { return input; }
    }
}