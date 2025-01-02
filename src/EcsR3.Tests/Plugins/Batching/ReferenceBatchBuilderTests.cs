using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Plugins.Batching.Builders;
using EcsR3.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsR3.Tests.Plugins.Batching
{
    public class ReferenceBatchBuilderTests
    {
        [Fact]
        public void should_create_batch_with_correct_values()
        {
            var fakeOne1 = new TestComponentOne {Data = "hello"};
            var fakeOne2 = new TestComponentOne {Data = "hi"};
            var fakeOnes = new[] {fakeOne1, fakeOne2};
            
            var fakeTwo1 = new TestComponentTwo {Data = "goodbye"};
            var fakeTwo2 = new TestComponentTwo {Data = "bye"};
            var fakeTwos = new[] {fakeTwo1, fakeTwo2};
            
            var mockComponentDatabase = Substitute.For<IComponentDatabase>();
            mockComponentDatabase.GetComponents<TestComponentOne>(Arg.Any<int>()).Returns(fakeOnes);
            mockComponentDatabase.GetComponents<TestComponentTwo>(Arg.Any<int>()).Returns(fakeTwos);
            
            var mockTypeLookup = Substitute.For<IComponentTypeLookup>();
            mockTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(0);
            mockTypeLookup.GetComponentTypeId(typeof(TestComponentTwo)).Returns(1);

            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity1.ComponentAllocations.Returns(new[] {0, 0});
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(2);
            fakeEntity2.ComponentAllocations.Returns(new[] {1, 1});
            
            var fakeEntities = new []{ fakeEntity1, fakeEntity2 };
            
            var batchBuilder = new ReferenceBatchBuilder<TestComponentOne, TestComponentTwo>(mockComponentDatabase, mockTypeLookup);

            var batches = batchBuilder.Build(fakeEntities);
                
            Assert.Equal(fakeEntities.Length, batches.Length);
            Assert.Equal(fakeEntities[0].Id, batches[0].EntityId);
            Assert.Equal(fakeOne1.Data, batches[0].Component1.Data);
            Assert.Equal(fakeTwo1.Data, batches[0].Component2.Data);
            Assert.Equal(fakeEntities[1].Id, batches[1].EntityId);
            Assert.Equal(fakeOne2.Data, batches[1].Component1.Data);
            Assert.Equal(fakeTwo2.Data, batches[1].Component2.Data);
        }
    }
}