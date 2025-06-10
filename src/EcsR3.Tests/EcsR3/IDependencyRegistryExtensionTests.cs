using System.Collections.Generic;
using System.Linq;
using EcsR3.Infrastructure;
using EcsR3.Tests.Systems.PriorityScenarios;
using NSubstitute;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Systems;
using Xunit;

namespace EcsR3.Tests.EcsR3
{
    public class IDependencyResolverExtensionsTests
    {
                [Fact]
        public void should_correctly_get_and_order_default_systems()
        {
            var defaultPrioritySystem = new DefaultPriorityGroupSystem();
            var defaultPrioritySetupSystem = new DefaultPrioritySetupSystem();
            var higherThanDefaultPrioritySystem = new HigherThanDefaultPriorityGroupSystem();
            var lowerThanDefaultPrioritySystem = new LowerThanDefaultPriorityGroupSystem();
            var lowPrioritySystem = new LowestPriorityGroupSystem();
            var lowPrioritySetupSystem = new LowestPrioritySetupSystem();
            var highPrioritySystem = new HighestPriorityGroupSystem();
            var highPrioritySetupSystem = new HighestPrioritySetupSystem();

            var systemList = new List<ISystem>
            {
                defaultPrioritySystem,
                higherThanDefaultPrioritySystem,
                lowerThanDefaultPrioritySystem,
                lowPrioritySystem,
                highPrioritySystem,
                defaultPrioritySetupSystem,
                lowPrioritySetupSystem,
                highPrioritySetupSystem
            };

            var mockResolver = Substitute.For<IDependencyResolver>();
            mockResolver.ResolveAll(typeof(ISystem)).Returns(systemList);

            var orderedSystems = IDependencyResolverSystemExtensions.GetAllBoundSystems(mockResolver).ToList();

            Assert.Equal(8, orderedSystems.Count);
            Assert.Equal(highPrioritySetupSystem, orderedSystems[0]);
            Assert.Equal(highPrioritySystem, orderedSystems[1]);
            Assert.Equal(higherThanDefaultPrioritySystem, orderedSystems[2]);
            Assert.True(orderedSystems[3] == defaultPrioritySetupSystem || orderedSystems[3] == defaultPrioritySystem);
            Assert.True(orderedSystems[4] == defaultPrioritySetupSystem || orderedSystems[4] == defaultPrioritySystem);
            Assert.Equal(lowerThanDefaultPrioritySystem, orderedSystems[5]);
            Assert.Equal(lowPrioritySystem, orderedSystems[6]);
            Assert.Equal(lowPrioritySetupSystem, orderedSystems[7]);
        }
    }
}