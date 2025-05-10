using SystemsR3.Systems.Conventional;
using SystemsR3.Tests.Models;

namespace SystemsR3.Tests.SystemsR3.Handlers.Helpers
{
    public interface ITestMultiReactToEvent : IReactToEventSystem<ComplexObject>, IReactToEventSystem<int>
    {
        
    }
}