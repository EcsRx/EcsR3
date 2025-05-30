using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections.Entities;
using EcsR3.Components.Lookups;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Tests.Models;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsR3.Observables;

public class ComputedComponentGroupTests
{
    [Fact]
    public void should_create_batch_with_correct_values()
    {
        var id1 = 1;
        var id2 = 2;
        var fakeEntities = new List<int>{ id1, id2 };
        
        var onChangedHandler = new Subject<IReadOnlyCollection<int>>();
        var dummyGroup = new LookupGroup([0,1], []);
        var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
        mockComputedEntityGroup.Count.Returns(fakeEntities.Count);
        mockComputedEntityGroup.Group.Returns(dummyGroup);
        mockComputedEntityGroup.OnChanged.Returns(onChangedHandler);
        mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());

        var mockTypeLookup = Substitute.For<IComponentTypeLookup>();
        mockTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(0);
        mockTypeLookup.GetComponentTypeId(typeof(TestComponentTwo)).Returns(1);

        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        mockEntityAllocationDatabase.GetEntityComponentAllocation(0, id1).Returns(22);
        mockEntityAllocationDatabase.GetEntityComponentAllocation(1, id1).Returns(23);
        mockEntityAllocationDatabase.GetEntityComponentAllocation(0, id2).Returns(1);
        mockEntityAllocationDatabase.GetEntityComponentAllocation(1, id2).Returns(2);
        
        var computedComponentGroup = new ComputedComponentGroup<TestComponentOne, TestComponentTwo>(mockTypeLookup, mockEntityAllocationDatabase, mockComputedEntityGroup);
        var batches = computedComponentGroup.Value.Span;
        Assert.Equal(fakeEntities.Count, batches.Length);
        Assert.Equal(fakeEntities[0], batches[0].EntityId);
        Assert.Equal(22, batches[0].Component1Allocation);
        Assert.Equal(23, batches[0].Component2Allocation);
        Assert.Equal(fakeEntities[1], batches[1].EntityId);
        Assert.Equal(1, batches[1].Component1Allocation);
        Assert.Equal(2, batches[1].Component2Allocation);
    }
}