using System.Collections.Generic;
using System.Linq;
using SystemsR3.Infrastructure;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Systems;
using NSubstitute;
using SystemsR3.Tests.Systems;
using Xunit;

namespace SystemsR3.Tests.SystemsRx
{
    public class IApplicationExtensionsTests
    {
        [Fact]
        public void should_correctly_order_default_systems()
        {

            var lowerThanDefaultPrioritySystem = new LowerThanDefaultPrioritySystem();
            var highPrioritySystem = new HighPrioritySystem();
            var highestPrioritySetupSystem = new HighestPrioritySystem();

            var systemList = new List<ISystem>
            {
                lowerThanDefaultPrioritySystem,
                highPrioritySystem,
                highestPrioritySetupSystem,
            };

            var mockContainer = Substitute.For<IDependencyResolver>();
            var mockApplication = Substitute.For<ISystemsR3Application>();
            mockContainer.ResolveAll(typeof(ISystem)).Returns(systemList);
            mockApplication.DependencyResolver.Returns(mockContainer);

            var orderedSystems = ISystemsRxApplicationExtensions.GetAllBoundSystems(mockApplication).ToList();

            Assert.Equal(3, orderedSystems.Count);
            Assert.Equal(highestPrioritySetupSystem, orderedSystems[0]);
            Assert.Equal(highPrioritySystem, orderedSystems[1]);
            Assert.Equal(lowerThanDefaultPrioritySystem, orderedSystems[2]);
        }
    }
}