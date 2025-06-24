using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections.Entities;
using EcsR3.Collections.Entities.Pools;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Entities.Routing;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Tests.Models;
using EcsR3.Tests.Systems.PriorityScenarios;
using NSubstitute;
using SystemsR3.Extensions;
using Xunit;

namespace EcsR3.Tests.EcsR3
{
    public class IEnumerableExtensionsTests
    {
        [Fact]
        public void should_correctly_order_priorities()
        {
            var defaultPrioritySystem = new DefaultPriorityGroupSystem();
            var higherThanDefaultPrioritySystem = new HigherThanDefaultPriorityGroupSystem();
            var lowerThanDefaultPrioritySystem = new LowerThanDefaultPriorityGroupSystem();
            var lowPrioritySystem = new LowestPriorityGroupSystem();
            var highPrioritySystem = new HighestPriorityGroupSystem();

            var systemList = new List<IGroupSystem>
            {
                defaultPrioritySystem,
                higherThanDefaultPrioritySystem,
                lowerThanDefaultPrioritySystem,
                lowPrioritySystem,
                highPrioritySystem
            };

            var orderedList = systemList.OrderByPriority().ToList();
            Assert.Equal(5, orderedList.Count);
            Assert.Equal(highPrioritySystem, orderedList[0]);
            Assert.Equal(higherThanDefaultPrioritySystem, orderedList[1]);
            Assert.Equal(defaultPrioritySystem, orderedList[2]);
            Assert.Equal(lowerThanDefaultPrioritySystem, orderedList[3]);
            Assert.Equal(lowPrioritySystem, orderedList[4]);
        }

        [Fact]
        public void should_correctly_get_applicable_systems()
        {
            var requiredComponents = new IComponent[] { new TestComponentOne(), new TestComponentTwo() };

            var applicableSystem1 = Substitute.For<IGroupSystem>();
            applicableSystem1.Group.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentTwo)));
            
            var notApplicableSystem1 = Substitute.For<IGroupSystem>();
            notApplicableSystem1.Group.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentThree)));

            var notApplicableSystem2 = Substitute.For<IGroupSystem>();
            notApplicableSystem2.Group.Returns(new Group(typeof(TestComponentTwo), typeof(TestComponentThree)));
            
            // Although this wants both 1 and 2, it also needs 3, which is not within the required components so shouldnt match
            var notApplicableSystem3 = Substitute.For<IGroupSystem>();
            notApplicableSystem3.Group.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentTwo), typeof(TestComponentThree)));

            var systemList = new List<IGroupSystem>
            {
                applicableSystem1,
                notApplicableSystem1,
                notApplicableSystem2,
                notApplicableSystem3,
            };
            
            var applicableSystems = systemList.GetApplicableSystems(requiredComponents);

            Assert.Equal(1, applicableSystems.Count());
            Assert.Contains(applicableSystem1, applicableSystems);
        }
    }
}