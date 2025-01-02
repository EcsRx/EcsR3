using SystemsR3.Attributes;
using SystemsR3.Systems;
using SystemsR3.Types;

namespace SystemsR3.Tests.Systems
{
    [Priority(PriorityTypes.High)]
    public class HighPrioritySystem : ISystem
    {
    }

    [Priority(PriorityTypes.Higher)]
    public class HighestPrioritySystem : ISystem
    {
    }
    
    [Priority(PriorityTypes.Lower)]
    public class LowerThanDefaultPrioritySystem : ISystem
    {
    }
}