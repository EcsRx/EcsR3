using EcsR3.Groups;
using EcsR3.Systems;
using SystemsR3.Attributes;

namespace EcsR3.Tests.Systems.PriorityScenarios
{
    [Priority(-1)]
    public class LowerThanDefaultPriorityGroupSystem : IGroupSystem
    {
        public IGroup Group => null;
    }
}