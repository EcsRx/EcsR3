using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using SystemsR3.Infrastructure;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Systems;
using SystemsR3.Tests.Systems;
using Xunit;

namespace SystemsR3.Tests.SystemsR3
{
    public class IDependencyResolverExtensionsTests
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

            var mockResolver = Substitute.For<IDependencyResolver>();
            mockResolver.ResolveAll(typeof(ISystem)).Returns(systemList);

            var orderedSystems = IDependencyResolverSystemExtensions.GetAllBoundSystems(mockResolver).ToList();

            Assert.Equal(3, orderedSystems.Count);
            Assert.Equal(highestPrioritySetupSystem, orderedSystems[0]);
            Assert.Equal(highPrioritySystem, orderedSystems[1]);
            Assert.Equal(lowerThanDefaultPrioritySystem, orderedSystems[2]);
        }
    }
}