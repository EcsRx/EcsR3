using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.HighPerformance;
using EcsR3.Collections.Entities;
using EcsR3.Collections.Entities.Pools;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
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
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);

        entityAllocationDatabase.ResizeAllEntityAllocations(resizeEntityCount);

        Assert.Equal(startingComponentCount, entityAllocationDatabase.ComponentAllocationData.GetLength(0));
        Assert.Equal(resizeEntityCount, entityAllocationDatabase.ComponentAllocationData.GetLength(1));

        for (var i = 0; i < entityAllocationDatabase.ComponentAllocationData.GetLength(0); i++)
        {
            for (var j = 0; j < entityAllocationDatabase.ComponentAllocationData.GetLength(1); j++)
            { Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[i, j]); }
        }
        
        Assert.True(entityAllocationDatabase.EntityCreationHashes.All(x => x == 0));
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
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);

        var allocations = new List<Entity>();
        for (var i = 0; i < allocationSize; i++)
        {
            var allocation = entityAllocationDatabase.AllocateEntity();
            allocations.Add(allocation);
        }

        Assert.Equal(allocationSize, allocations.Count);
        Assert.Equal(allocationSize, allocations.Distinct().Count());
    }

    [Fact]
    public void should_allocate_entities_with_specific_ids_regardless_of_pool_size()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool(new PoolConfig(2, 10));
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockCreationHasher.GenerateHash().Returns(123);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);
        
        var entityIdInsidePool = entityAllocationDatabase.AllocateEntity(1);
        var entityIdOutsidePool = entityAllocationDatabase.AllocateEntity(10000);

        Assert.Equal(1, entityIdInsidePool.Id);
        Assert.Equal(123, entityIdInsidePool.CreationHash);
        Assert.Equal(10000, entityIdOutsidePool.Id);
        Assert.Equal(123, entityIdOutsidePool.CreationHash);
        
        var expectedExpansionSize = 10000 + entityIdPool.PoolConfig.ExpansionSize + 1;
        Assert.Equal(expectedExpansionSize, entityIdPool.Size);
        Assert.Equal(expectedExpansionSize + 1, entityAllocationDatabase.EntityLength);
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
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);

        // Ignore NoAllocation we are bypassing normal entity creation route
        var entity = new Entity(5, 0);
        
        entityAllocationDatabase.ComponentAllocationData = new int[componentTypeIds.Length, 10];
        new Span2D<int>(entityAllocationDatabase.ComponentAllocationData).Fill(IEntityAllocationDatabase.NoAllocation);
        
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity.Id] = 1;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entity.Id] = 1;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity.Id] = 1;
        entityAllocationDatabase.ReleaseEntity(entity);

        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity.Id]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entity.Id]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity.Id]);
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
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);

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
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);

        var entity = new Entity(3, 0);
        
        entityAllocationDatabase.ComponentAllocationData = new int[componentTypeIds.Length, 10];
        new Span2D<int>(entityAllocationDatabase.ComponentAllocationData).Fill(IEntityAllocationDatabase.NoAllocation);
        
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity.Id] = 22;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity.Id] = 23;
        var componentTypes = entityAllocationDatabase.GetAllocatedComponentTypes(entity);

        Assert.Equal(2, componentTypes.Length);
        Assert.Contains(componentTypeId1, componentTypes);
        Assert.Contains(componentTypeId3, componentTypes);
    }
    
    [Fact]
    public void should_return_null_when_entity_doesnt_exist()
    {
        // Make our starting pool 2
        var entityIdPool = new EntityIdPool(new PoolConfig(2,10));
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);

        var entityInsidePoolRange = entityAllocationDatabase.GetEntity(1);
        var entityOutsidePoolRange = entityAllocationDatabase.GetEntity(10000);

        Assert.False(entityInsidePoolRange.HasValue);
        Assert.False(entityOutsidePoolRange.HasValue);
    }
    
    [Fact]
    public void should_get_all_components_allocations_for_entity_correctly()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);

        var entity = new Entity(3, 0);
        
        entityAllocationDatabase.ComponentAllocationData = new int[componentTypeIds.Length, 10];
        new Span2D<int>(entityAllocationDatabase.ComponentAllocationData).Fill(IEntityAllocationDatabase.NoAllocation);
        
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity.Id] = 22;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity.Id] = 23;
        var allocations = entityAllocationDatabase.GetEntityAllocations(entity);

        Assert.Equal(3, allocations.Length);
        Assert.Equal(22, allocations[componentTypeId1]);
        Assert.Equal(-1, allocations[componentTypeId2]);
        Assert.Equal(23, allocations[componentTypeId3]);
    }
    
    [Fact]
    public void should_get_entity_component_allocation()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);
        
        var entity = new Entity(3, 0);
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity.Id] = 22;
        
        var allocation1 = entityAllocationDatabase.GetEntityComponentAllocation(componentTypeId1, entity);
        var allocation2 = entityAllocationDatabase.GetEntityComponentAllocation(componentTypeId2, entity);
            
        Assert.Equal(22, allocation1);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, allocation2);
    }
    
    [Fact]
    public void should_allocate_components_individually_correctly()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);

        mockComponentDatabase.Allocate(componentTypeId1).Returns(22);
        mockComponentDatabase.Allocate(componentTypeId2).Returns(51);
        mockComponentDatabase.Allocate(componentTypeId3).Returns(74);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);

        var entity = new Entity(3, 0);
        var allocation1 = entityAllocationDatabase.AllocateComponent(componentTypeId1, entity);
        var allocation2 = entityAllocationDatabase.AllocateComponent(componentTypeId3, entity);

        Assert.Equal(22, allocation1);
        Assert.Equal(74, allocation2);
        Assert.Equal(22, entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity.Id]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entity.Id]);
        Assert.Equal(74, entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity.Id]);
    }
    
    [Fact]
    public void should_allocate_component_to_multiple_entities_correctly()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);

        var expectedC1Allocations = new[]{ 22, 24 };
        var expectedC2Allocations = new[]{ 51, 54 };
        var expectedC3Allocations = new[]{ 77, 79 };
        mockComponentDatabase.Allocate(componentTypeId1, 2).Returns(expectedC1Allocations);
        mockComponentDatabase.Allocate(componentTypeId2, 2).Returns(expectedC2Allocations);
        mockComponentDatabase.Allocate(componentTypeId3, 2).Returns(expectedC3Allocations);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);

        var entity1 = new Entity(3, 0);
        var entity2 = new Entity(6, 0);
        var entityIds = new[] { entity1, entity2 };
        var component1Allocations = entityAllocationDatabase.AllocateComponent(componentTypeId1, entityIds);
        var component3Allocations = entityAllocationDatabase.AllocateComponent(componentTypeId3, entityIds);

        Assert.Equal(entityIds.Length, component1Allocations.Length);
        Assert.Equal(expectedC1Allocations, component1Allocations);
        
        Assert.Equal(entityIds.Length, component3Allocations.Length);
        Assert.Equal(expectedC3Allocations, component3Allocations);
        
        Assert.Equal(expectedC1Allocations[0], entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity1.Id]);
        Assert.Equal(expectedC1Allocations[1], entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity2.Id]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entity1.Id]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entity2.Id]);
        Assert.Equal(expectedC3Allocations[0], entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity1.Id]);
        Assert.Equal(expectedC3Allocations[1], entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity2.Id]);
    }
    
    [Fact]
    public void should_batch_allocate_components_correctly()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);

        mockComponentDatabase.Allocate(componentTypeId1).Returns(22);
        mockComponentDatabase.Allocate(componentTypeId2).Returns(51);
        mockComponentDatabase.Allocate(componentTypeId3).Returns(74);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);

        var entity = new Entity(3, 0);
        var allocations = entityAllocationDatabase.AllocateComponents([componentTypeId1, componentTypeId3], entity);

        Assert.Equal(22, allocations[0]);
        Assert.Equal(74, allocations[1]);
        Assert.Equal(22, entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity.Id]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entity.Id]);
        Assert.Equal(74, entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity.Id]);
    }
    
    [Fact]
    public void should_return_if_entity_has_component_or_not()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);
        
        var entity = new Entity(3, 0);
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity.Id] = 22;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity.Id] = 13;
        
        var hasComponent1 = entityAllocationDatabase.HasComponent(componentTypeId1, entity);
        var hasComponent2 = entityAllocationDatabase.HasComponent(componentTypeId2, entity);
        var hasComponent3 = entityAllocationDatabase.HasComponent(componentTypeId3, entity);
            
        Assert.True(hasComponent1);
        Assert.False(hasComponent2);
        Assert.True(hasComponent3);
    }
    
    [Fact]
    public void should_release_component_on_entity_correctly()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);
        
        var entity = new Entity(3, 0);
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity.Id] = 22;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity.Id] = 13;
        
        entityAllocationDatabase.ReleaseComponent(componentTypeId1, entity);
            
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity.Id]);
        Assert.Equal(13, entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity.Id]);
    }
    
    [Fact]
    public void should_batch_release_component_on_entity_correctly()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);
        
        var entity1 = new Entity(3, 0);
        var entity2 = new Entity(6, 0);
        var entities = new[] { entity1, entity2 };
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity1.Id] = 22;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity1.Id] = 13;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity2.Id] = 25;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entity2.Id] = 16;
        
        entityAllocationDatabase.ReleaseComponent(componentTypeId1, entities);
        entityAllocationDatabase.ReleaseComponent(componentTypeId3, entities);
            
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity1.Id]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity2.Id]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity1.Id]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity2.Id]);
        Assert.Equal(IEntityAllocationDatabase.NoAllocation, entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entity1.Id]);
        Assert.Equal(16, entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entity2.Id]);
    }
    
    [Fact]
    public void should_get_all_entities_with_a_component()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);

        var entity1 = new Entity(3, 0);
        var entity2 = new Entity(6, 0);
        var entity3 = new Entity(7, 0);
        var expectedEntities = new[] { entity1, entity2 };
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity1.Id] = 22;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity1.Id] = 13;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity2.Id] = 25;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entity2.Id] = 16;
        
        var actualEntities = entityAllocationDatabase.GetEntitiesWithComponent(componentTypeId1);
            
        Assert.Equal(expectedEntities, actualEntities);
    }
        
    [Fact]
    public void should_get_all_entities_with_components()
    {
        // Easier to just have a real one of these
        var entityIdPool = new EntityIdPool();
        
        var componentTypeId1 = 0;
        var componentTypeId2 = 1;
        var componentTypeId3 = 2;
        var componentTypeIds = new[] { componentTypeId1, componentTypeId2, componentTypeId3 };
        
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        var mockCreationHasher = Substitute.For<ICreationHasher>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(componentTypeIds);
        
        var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, mockComponentDatabase,
            mockComponentTypeLookup, mockCreationHasher);
        
        var entity1Id = 3;
        var entity2Id = 6;
        var entity3Id = 7;
        var entity4Id = 8;
        var expectedEntities = new[] { entity1Id, entity4Id };
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity1Id] = 22;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity1Id] = 13;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity2Id] = 25;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId2, entity2Id] = 16;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity3Id] = 13;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId1, entity4Id] = 66;
        entityAllocationDatabase.ComponentAllocationData[componentTypeId3, entity4Id] = 71;
        
        var actualEntities = entityAllocationDatabase.GetEntitiesWithComponents(new[] { componentTypeId1, componentTypeId3 });
            
        Assert.Equal(expectedEntities, actualEntities);
    }
}