using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.HighPerformance;
using EcsR3.Collections.Entities;
using EcsR3.Collections.Entities.Pools;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities.Routing;
using NSubstitute;
using SystemsR3.Pools.Config;
using Xunit;

namespace EcsR3.Tests.EcsR3.Entities;

public class EntityAllocationDatabaseTests
{
    [Theory]
    [InlineData(1,1,10)]
    [InlineData(5,10,20)]
    [InlineData(5,50,100)]
    public void should_resize_correctly(int startingComponentCount, int startingEntityCount, int resizeEntityCount)
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool(new PoolConfig(startingEntityCount, resizeEntityCount));

        var componentTypeIds =Enumerable.Range(0, startingComponentCount).ToArray();
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockEntityChangeRouter, mockComponentTypeLookup);

        entityAllocationDatabase.ResizeAllEntityAllocations(resizeEntityCount);

        Assert.Equal(startingComponentCount, entityAllocationDatabase.ComponentAllocationData.GetLength(0));
        Assert.Equal(resizeEntityCount, entityAllocationDatabase.ComponentAllocationData.GetLength(1));

        for (var i = 0; i < entityAllocationDatabase.ComponentAllocationData.GetLength(0); i++)
        {
            for (var j = 0; j < entityAllocationDatabase.ComponentAllocationData.GetLength(1); j++)
            { Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[i, j]); }
        }
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(1026)]
    public void should_allocate_entities_individually_correctly(int allocationSize)
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockEntityChangeRouter, mockComponentTypeLookup);

        var allocations = new List<int>();
        for (var i = 0; i < allocationSize; i++)
        {
            var allocation = entityAllocationDatabase.AllocateEntity();
            allocations.Add(allocation);
        }

        Assert.Equal(allocationSize, allocations.Count);
        Assert.Equal(allocationSize, allocations.Distinct().Count());
    }

    [Fact]
    public void should_release_entity_correctly()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockEntityChangeRouter, mockComponentTypeLookup);

        var entityId = 5;
        
        entityAllocationDatabase.ComponentAllocationData = new int[componentTypeIds.Length, 10];
        new Span2D<int>(entityAllocationDatabase.ComponentAllocationData).Fill(IEntityAllocationDatabase.NoAllocation);
        
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entityId] = 1;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entityId] = 1;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entityId] = 1;
        entityAllocationDatabase.ReleaseEntity(entityId);

        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entityId]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entityId]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entityId]);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(1026)]
    public void should_allocate_entities_batched_correctly(int allocationSize)
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockEntityChangeRouter, mockComponentTypeLookup);

        var allocations = entityAllocationDatabase.AllocateEntities(allocationSize);

        Assert.Equal(allocationSize, allocations.Length);
        Assert.Equal(allocationSize, allocations.Distinct().Count());
    }
    
    [Fact]
    public void should_get_entity_components_correctly()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockEntityChangeRouter, mockComponentTypeLookup);

        var entityId = 3;
        
        entityAllocationDatabase.ComponentAllocationData = new int[componentTypeIds.Length, 10];
        new Span2D<int>(entityAllocationDatabase.ComponentAllocationData).Fill(IEntityAllocationDatabase.NoAllocation);
        
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entityId] = 22;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entityId] = 23;
        var componentTypes = entityAllocationDatabase.GetAllocatedComponentTypes(entityId);

        Assert.Equal(2, componentTypes.Length);
        Assert.Contains(componentTypeId1, componentTypes);
        Assert.Contains(componentTypeId3, componentTypes);
    }
    
    [Fact]
    public void should_get_entity_components_allocations_correctly()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockEntityChangeRouter, mockComponentTypeLookup);

        var entityId = 3;
        
        entityAllocationDatabase.ComponentAllocationData = new int[componentTypeIds.Length, 10];
        new Span2D<int>(entityAllocationDatabase.ComponentAllocationData).Fill(IEntityAllocationDatabase.NoAllocation);
        
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entityId] = 22;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entityId] = 23;
        var allocations = entityAllocationDatabase.GetEntityAllocations(entityId);

        Assert.Equal(3, allocations.Length);
        Assert.Equal(22, allocations[componentTypeId1]);
        Assert.Equal(-1, allocations[componentTypeId2]);
        Assert.Equal(23, allocations[componentTypeId3]);
    }
}