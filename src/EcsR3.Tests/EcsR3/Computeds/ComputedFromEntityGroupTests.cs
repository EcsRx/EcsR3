using System.Collections.Generic;
using System.Linq;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Tests.EcsR3.Computeds.Models;
using EcsR3.Tests.Models;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsR3.Computeds
{
    public class ComputedFromEntityGroupTests
    {
        [Fact]
        public void should_refresh_when_data_changed()
        {
            var entity1 = new Entity(1, 0);
            var entity2 = new Entity(2, 0);
            var entity3 = new Entity(3, 0);
            var fakeEntities = new List<Entity> {entity1, entity2, entity3};
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            entityComponentAccessor.HasComponent<TestComponentThree>(Arg.Is(entity1)).Returns(false);
            entityComponentAccessor.HasComponent<TestComponentThree>(Arg.Is(entity2)).Returns(true);
            entityComponentAccessor.HasComponent<TestComponentThree>(Arg.Is(entity3)).Returns(true);

            var expectedData = entity3.GetHashCode();
            
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());

            var computedGroupData = new TestComputedFromEntityGroup(entityComponentAccessor, mockComputedEntityGroup);

            fakeEntities.Remove(entity2);
            computedGroupData.ManuallyRefresh.OnNext(Unit.Default);
            
            var actualData = computedGroupData.Value;
            Assert.Equal(expectedData, actualData);         
        }
    }
}