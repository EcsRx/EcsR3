using System.Collections.Generic;
using System.Linq;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
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

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2, fakeEntity3};
            
            var mockObservableGroup = Substitute.For<IComputedEntityGroup>();
            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedFromEntityGroup(mockObservableGroup);

            fakeEntities.Remove(fakeEntity2);
            computedGroupData.ManuallyRefresh.OnNext(Unit.Default);
            
            var actualData = computedGroupData.Value;
            Assert.Equal(expectedData, actualData);         
        }
    }
}