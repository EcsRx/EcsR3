using SystemsR3.Tests.Models;
using SystemsR3.Systems.Conventional;

namespace SystemsR3.Tests.SystemsRx.Handlers.Helpers
{
    public interface ITestMultiReactToEvent : IReactToEventSystem<ComplexObject>, IReactToEventSystem<int>
    {
        
    }
}