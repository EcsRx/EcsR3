using System.Collections.Generic;
using System.Linq;
using EcsR3.Infrastructure;
using EcsR3.Infrastructure.Extensions;
using EcsR3.Tests.Systems.PriorityScenarios;
using NSubstitute;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Systems;
using Xunit;
using ViewApplicationExtensions =  EcsR3.Plugins.Views.Extensions.IEcsRxApplicationExtensions;


namespace EcsR3.Tests.EcsR3
{
    public class IApplicationExtensionsTests
    {
        [Fact]
        public void should_correctly_order_reactive_systems()
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
            var mockApplication = Substitute.For<IEcsR3Application>();
            mockResolver.ResolveAll(typeof(ISystem)).Returns(systemList);
            mockApplication.DependencyResolver.Returns(mockResolver);

            var orderedSystems = IEcsRxApplicationExtensions.GetAllBoundReactiveSystems(mockApplication).ToList();

            Assert.Equal(8, orderedSystems.Count);
            Assert.Equal(highPrioritySetupSystem, orderedSystems[0]);
            Assert.Equal(defaultPrioritySetupSystem, orderedSystems[1]);
            Assert.Equal(lowPrioritySetupSystem, orderedSystems[2]);
            Assert.Equal(highPrioritySystem, orderedSystems[3]);
            Assert.Equal(higherThanDefaultPrioritySystem, orderedSystems[4]);
            Assert.Equal(defaultPrioritySystem, orderedSystems[5]);
            Assert.Equal(lowerThanDefaultPrioritySystem, orderedSystems[6]);
            Assert.Equal(lowPrioritySystem, orderedSystems[7]);
        }
        
        [Fact]
        public void should_correctly_order_view_systems()
        {
            var defaultPrioritySystem = new DefaultPriorityGroupSystem();
            var defaultPrioritySetupSystem = new DefaultPrioritySetupSystem();
            var higherThanDefaultPrioritySystem = new HigherThanDefaultPriorityGroupSystem();
            var lowerThanDefaultPrioritySystem = new LowerThanDefaultPriorityGroupSystem();
            var lowPrioritySystem = new LowestPriorityGroupSystem();
            var lowPrioritySetupSystem = new LowestPrioritySetupSystem();
            var highPrioritySystem = new HighestPriorityGroupSystem();
            var highPrioritySetupSystem = new HighestPrioritySetupSystem();
            var defaultPriorityViewSystem = new DefaultPriorityViewResolverSystem();
            var highestPriorityViewSystem = new HighestPriorityViewResolverSystem();
            var lowestPriorityViewSystem = new LowestPriorityViewResolverSystem();

            var systemList = new List<ISystem>
            {
                defaultPrioritySystem,
                higherThanDefaultPrioritySystem,
                lowerThanDefaultPrioritySystem,
                lowPrioritySystem,
                highPrioritySystem,
                defaultPrioritySetupSystem,
                lowPrioritySetupSystem,
                highPrioritySetupSystem,
                defaultPriorityViewSystem,
                highestPriorityViewSystem,
                lowestPriorityViewSystem
            };

            var mockResolver = Substitute.For<IDependencyResolver>();
            var mockApplication = Substitute.For<IEcsR3Application>();
            mockResolver.ResolveAll(typeof(ISystem)).Returns(systemList);
            mockApplication.DependencyResolver.Returns(mockResolver);

            var orderedSystems = ViewApplicationExtensions.GetAllBoundViewSystems(mockApplication).ToList();

            Assert.Equal(11, orderedSystems.Count);
            Assert.Equal(highPrioritySetupSystem, orderedSystems[0]);
            Assert.Equal(defaultPrioritySetupSystem, orderedSystems[1]);
            Assert.Equal(lowPrioritySetupSystem, orderedSystems[2]);
            Assert.Equal(highestPriorityViewSystem, orderedSystems[3]);
            Assert.Equal(defaultPriorityViewSystem, orderedSystems[4]);
            Assert.Equal(lowestPriorityViewSystem, orderedSystems[5]);
            Assert.Equal(highPrioritySystem, orderedSystems[6]);
            Assert.Equal(higherThanDefaultPrioritySystem, orderedSystems[7]);
            Assert.Equal(defaultPrioritySystem, orderedSystems[8]);
            Assert.Equal(lowerThanDefaultPrioritySystem, orderedSystems[9]);
            Assert.Equal(lowPrioritySystem, orderedSystems[10]);
        }
    }
}