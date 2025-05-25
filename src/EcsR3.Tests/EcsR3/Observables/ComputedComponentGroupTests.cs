using System.Collections.Generic;
using System.Linq;
using EcsR3.Components.Lookups;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Tests.Models;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Extensions;
using NSubstitute.ReturnsExtensions;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsR3.Observables;

public class ComputedComponentGroupTests
{
    [Fact]
    public void should_create_batch_with_correct_values()
    {
        var fakeEntity1 = Substitute.For<IEntity>();
        fakeEntity1.Id.Returns(1);
        fakeEntity1.ComponentAllocations.Returns([22, 23]);
        
        var fakeEntity2 = Substitute.For<IEntity>();
        fakeEntity2.Id.Returns(2);
        fakeEntity2.ComponentAllocations.Returns([1, 2]);
        
        var fakeEntities = new []{ fakeEntity1, fakeEntity2 };
        
        var onChangedHandler = new Subject<IReadOnlyCollection<IEntity>>();
        var dummyGroup = new LookupGroup([0,1], []);
        var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
        mockComputedEntityGroup.Count.Returns(fakeEntities.Length);
        mockComputedEntityGroup.Group.Returns(dummyGroup);
        mockComputedEntityGroup.OnChanged.Returns(onChangedHandler);
        mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.Select(x => x).GetEnumerator());

        var mockTypeLookup = Substitute.For<IComponentTypeLookup>();
        mockTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(0);
        mockTypeLookup.GetComponentTypeId(typeof(TestComponentTwo)).Returns(1);
        
        var computedComponentGroup = new ComputedComponentGroup<TestComponentOne, TestComponentTwo>(mockTypeLookup, mockComputedEntityGroup);
        var batches = computedComponentGroup.Value;
        Assert.Equal(fakeEntities.Length, batches.Length);
        Assert.Equal(fakeEntities[0].Id, batches[0].EntityId);
        Assert.Equal(22, batches[0].Component1Allocation);
        Assert.Equal(23, batches[0].Component2Allocation);
        Assert.Equal(fakeEntities[1].Id, batches[1].EntityId);
        Assert.Equal(1, batches[1].Component1Allocation);
        Assert.Equal(2, batches[1].Component2Allocation);
    }
}