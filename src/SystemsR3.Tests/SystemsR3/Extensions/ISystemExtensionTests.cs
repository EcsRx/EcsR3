using System.Linq;
using R3;
using SystemsR3.Extensions;
using SystemsR3.Scheduling;
using SystemsR3.Systems;
using SystemsR3.Systems.Conventional;
using Xunit;
using Xunit.Sdk;

namespace SystemsR3.Tests.SystemsR3.Extensions;

public class ISystemExtensionTests
{
        class TestSystemClass : IBasicSystem, IManualSystem, IReactToEventSystem<object>, IReactToEventSystem<int>
        {
                public void Execute(ElapsedTime elapsedTime)
                {
                        throw new System.NotImplementedException();
                }

                public void StartSystem()
                {
                        throw new System.NotImplementedException();
                }

                public void StopSystem()
                {
                        throw new System.NotImplementedException();
                }

                public Observable<object> ObserveOn(Observable<object> observable)
                {
                        throw new System.NotImplementedException();
                }

                public void Process(object eventData)
                {
                        throw new System.NotImplementedException();
                }

                public Observable<int> ObserveOn(Observable<int> observable)
                {
                        throw new System.NotImplementedException();
                }

                public void Process(int eventData)
                {
                        throw new System.NotImplementedException();
                }
        }
        
        [Fact]
        public void should_get_all_interfaces_implemented_by_system()
        {
                var system = new TestSystemClass();
                var implementedSystems = system.GetSystemTypesImplemented().ToArray();
                Assert.Equal(4, implementedSystems.Length);
        }
}