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
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = fakeEntity3.GetHashCode();

            var fakeEntities = new List<int> {fakeEntity1.Id, fakeEntity2.Id, fakeEntity3.Id};
            
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());

            var mockAccessor = Substitute.For<IEntityComponentAccessor>();
            
            var computedGroupData = new TestComputedFromEntityGroup(mockAccessor, mockComputedEntityGroup);

            fakeEntities.Remove(fakeEntity2.Id);
            computedGroupData.ManuallyRefresh.OnNext(Unit.Default);
            
            var actualData = computedGroupData.Value;
            Assert.Equal(expectedData, actualData);         
        }
    }
}