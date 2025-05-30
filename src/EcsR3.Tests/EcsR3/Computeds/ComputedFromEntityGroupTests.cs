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
            var id1 = 1;
            var id2 = 2;
            var id3 = 3;
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            entityComponentAccessor.HasComponent<TestComponentThree>(Arg.Is(id1)).Returns(false);
            entityComponentAccessor.HasComponent<TestComponentThree>(Arg.Is(id2)).Returns(true);
            entityComponentAccessor.HasComponent<TestComponentThree>(Arg.Is(id3)).Returns(true);

            var expectedData = id3.GetHashCode();

            var fakeEntities = new List<int> {id1, id2, id3};
            
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());

            var computedGroupData = new TestComputedFromEntityGroup(entityComponentAccessor, mockComputedEntityGroup);

            fakeEntities.Remove(id2);
            computedGroupData.ManuallyRefresh.OnNext(Unit.Default);
            
            var actualData = computedGroupData.Value;
            Assert.Equal(expectedData, actualData);         
        }
    }
}