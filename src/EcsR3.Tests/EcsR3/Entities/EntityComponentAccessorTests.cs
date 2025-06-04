using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections.Entities;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Entities.Routing;
using EcsR3.Tests.Models;
using NSubstitute;
using SystemsR3.Utility;
using Xunit;

namespace EcsR3.Tests.EcsR3.Entities;

public class EntityComponentAccessorTests
{
    [Fact]
    public void should_add_component_correctly()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var componentTypeIds = new []{1};
        var allocationIndexes = new []{2};
        var dummyEntity = new Entity(1, 0);
        var components = new IComponent [] { new TestComponentOne() };
        
        mockComponentTypeLookup.GetComponentTypeIds(components).Returns(componentTypeIds);
        mockEntityAllocationDatabase.AllocateComponents(componentTypeIds, dummyEntity).Returns(allocationIndexes);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        entityComponentAccessor.AddComponents(dummyEntity, components);
        
        mockEntityAllocationDatabase.Received(1).AllocateComponents(componentTypeIds, dummyEntity);
        mockComponentDatabase.Received(1).SetMany(componentTypeIds, allocationIndexes, components);
        mockEntityChangeRouter.Received(1).PublishEntityAddedComponents(dummyEntity, componentTypeIds);
    }
    
    [Fact(Skip = "NSubstitute doesn't support refs")]
    public void should_create_component_correctly()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var componentTypeId = 1;
        var allocationIndex = 2;
        var dummyEntity = new Entity(1, 0);
        
        mockComponentTypeLookup.GetComponentTypeId(typeof(TestStructComponentOne)).Returns(componentTypeId);
        mockEntityAllocationDatabase.AllocateComponent(componentTypeId, dummyEntity).Returns(allocationIndex);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        entityComponentAccessor.CreateComponent<TestStructComponentOne>(dummyEntity);
        
        mockEntityAllocationDatabase.Received(1).AllocateComponent(componentTypeId, dummyEntity);
        mockComponentDatabase.Received(1).Set(componentTypeId, allocationIndex, Arg.Any<TestStructComponentOne>());
        mockEntityChangeRouter.Received(1).PublishEntityAddedComponents(dummyEntity, [componentTypeId]);
    }
    
    [Fact]
    public void should_batch_create_component_correctly()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var componentTypeId = 1;
        var allocationIndexes = new []{2};
        var dummyEntity = new Entity(1, 0);
        var entities = new[] { dummyEntity };
        
        mockComponentTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(componentTypeId);
        mockEntityAllocationDatabase.AllocateComponent(componentTypeId, entities).Returns(allocationIndexes);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        entityComponentAccessor.CreateComponent<TestComponentOne>(entities);
        
        mockEntityAllocationDatabase.Received(1).AllocateComponent(componentTypeId, entities);
        mockComponentDatabase.Received(1).Set(componentTypeId, allocationIndexes, Arg.Any<TestComponentOne[]>());
        mockEntityChangeRouter.Received(1).PublishEntityAddedComponents(entities, Arg.Is<int[]>(x => x.Length == 1 && x.Contains(componentTypeId)));
    }
    
    [Fact]
    public void should_remove_components_correctly()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var componentTypeId = 1;
        var allocationIndex = 2;
        var dummyEntity = new Entity(1, 0);
        
        mockComponentTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(componentTypeId);
        mockEntityAllocationDatabase.HasComponent(componentTypeId, dummyEntity).Returns(true);
        mockEntityAllocationDatabase.ReleaseComponent(componentTypeId, dummyEntity).Returns(allocationIndex);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        entityComponentAccessor.RemoveComponents(dummyEntity, [componentTypeId]);
        
        mockEntityAllocationDatabase.Received(1).ReleaseComponent(componentTypeId, dummyEntity);
        mockComponentDatabase.Received(1).Remove(componentTypeId, allocationIndex);
        mockEntityChangeRouter.Received(1).PublishEntityRemovingComponents(dummyEntity, Arg.Is<int[]>(x => x.Length == 1 && x.Contains(componentTypeId)));
        mockEntityChangeRouter.Received(1).PublishEntityRemovedComponents(dummyEntity, Arg.Is<int[]>(x => x.Length == 1 && x.Contains(componentTypeId)));
    }
    
    [Fact]
    public void should_get_components_correctly()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var componentTypeId = 0;
        var allocationIndexes = new []{2};
        var dummyEntity = new Entity(1, 0);
        var dummyComponent = new TestComponentOne();
        
        mockComponentTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(componentTypeId);
        mockEntityAllocationDatabase.GetEntityAllocations(dummyEntity).Returns(allocationIndexes);
        mockComponentDatabase.Get(componentTypeId, allocationIndexes[0]).Returns(dummyComponent);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        var components = entityComponentAccessor.GetComponents(dummyEntity).ToArray();
        
        mockEntityAllocationDatabase.Received(1).GetEntityAllocations(dummyEntity);
        mockComponentDatabase.Received(1).Get(componentTypeId, allocationIndexes[0]);

        Assert.Single(components);
        Assert.Contains(dummyComponent, components);
    }
    
    [Fact]
    public void should_get_component_if_exists_correctly()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var componentTypeId = 1;
        var allocationIndex = 2;
        var dummyEntity = new Entity(1, 0);
        var dummyComponent = new TestComponentOne();
        
        mockComponentTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(componentTypeId);
        mockEntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, dummyEntity).Returns(allocationIndex);
        mockComponentDatabase.Get(componentTypeId, allocationIndex).Returns(dummyComponent);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        var component = entityComponentAccessor.GetComponent(dummyEntity, typeof(TestComponentOne));
        
        mockEntityAllocationDatabase.Received(1).GetEntityComponentAllocation(componentTypeId, dummyEntity);
        mockComponentDatabase.Received(1).Get(componentTypeId, allocationIndex);

        Assert.NotNull(component);
        Assert.Equal(dummyComponent, component);
    }
    
    [Fact]
    public void should_throw_exception_for_get_component_if_doesnt_exist()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var componentTypeId = 1;
        var allocationIndex = 2;
        var dummyEntity = new Entity(1, 0);
        var dummyComponent = new TestComponentOne();
        
        mockComponentTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(componentTypeId);
        mockEntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, dummyEntity).Returns(IEntityAllocationDatabase.NoAllocation);
        mockComponentDatabase.Get(componentTypeId, allocationIndex).Returns(dummyComponent);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        Assert.Throws<Exception>(() =>
        {
            entityComponentAccessor.GetComponent(dummyEntity, typeof(TestComponentOne));
        });
    }
    
        
    [Fact]
    public void should_batch_get_component_if_exists_correctly()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var componentTypeId = 1;
        var allocationIndexes = new[]{2};
        var dummyEntity = new Entity(1, 0);
        var entities = new[] { dummyEntity };
        var dummyComponent = new TestComponentOne();
        
        mockComponentTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(componentTypeId);
        mockEntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, entities).Returns(allocationIndexes);
        mockComponentDatabase.Get<TestComponentOne>(componentTypeId, allocationIndexes).Returns([dummyComponent]);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        var components = entityComponentAccessor.GetComponent<TestComponentOne>(entities);
        
        mockEntityAllocationDatabase.Received(1).GetEntityComponentAllocation(componentTypeId, entities);
        mockComponentDatabase.Received(1).Get<TestComponentOne>(componentTypeId, allocationIndexes);

        Assert.Single(components);
        Assert.Contains(dummyComponent, components);
    }
    
    [Fact]
    public void should_throw_exception_for_batch_get_component_if_doesnt_exist()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var componentTypeId = 1;
        var allocationIndex = 2;
        var dummyEntity = new Entity(1, 0);
        var entities = new[] { dummyEntity };
        var dummyComponent = new TestComponentOne();
        
        mockComponentTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(componentTypeId);
        mockEntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, entities).Returns([IEntityAllocationDatabase.NoAllocation]);
        mockComponentDatabase.Get(componentTypeId, allocationIndex).Returns(dummyComponent);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        Assert.Throws<Exception>(() =>
        {
            entityComponentAccessor.GetComponent<TestComponentOne>(entities);
        });
    }
    
    [Fact]
    public void should_get_component_ref_buffer_correctly()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var componentTypeId = 1;
        var allocationIndexes = new[]{2};
        var dummyEntity = new Entity(1, 0);
        var entities = new[] { dummyEntity };
        var dummyComponent = new TestStructComponentOne();
        var dummyComponentPoolData = new[] { dummyComponent };
        
        mockComponentTypeLookup.GetComponentTypeId(typeof(TestStructComponentOne)).Returns(componentTypeId);
        mockEntityAllocationDatabase.GetEntityComponentAllocation(componentTypeId, entities).Returns(allocationIndexes);
        mockComponentDatabase.GetRef<TestStructComponentOne>(componentTypeId, allocationIndexes).Returns(new RefBuffer<TestStructComponentOne>(dummyComponentPoolData, [0]));
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        var refBuffer = entityComponentAccessor.GetComponentRef<TestStructComponentOne>(entities);
        
        mockEntityAllocationDatabase.Received(1).GetEntityComponentAllocation(componentTypeId, entities);
        mockComponentDatabase.Received(1).GetRef<TestStructComponentOne>(componentTypeId, allocationIndexes);

        Assert.Equal(1, refBuffer.Count);
    }
    
        
    [Fact]
    public void should_return_true_when_validating_valid_entity()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var dummyEntity = new Entity(1, 0);
        
        mockEntityAllocationDatabase.GetEntity(dummyEntity.Id).Returns(dummyEntity);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        var isValid = entityComponentAccessor.IsEntityValid(dummyEntity);
        
        mockEntityAllocationDatabase.Received(1).GetEntity(dummyEntity.Id);

        Assert.True(isValid);
    }
    
    [Fact]
    public void should_return_false_when_validating_entity_with_invalid_id()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var dummyEntity = new Entity(-1, 0);
        
        mockEntityAllocationDatabase.GetEntity(dummyEntity.Id).Returns(dummyEntity);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        var isValid = entityComponentAccessor.IsEntityValid(dummyEntity);
        
        mockEntityAllocationDatabase.Received(0).GetEntity(dummyEntity.Id);

        Assert.False(isValid);
    }
    
    [Fact]
    public void should_return_false_when_validating_entity_with_stale_creation_hash()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var staleEntity = new Entity(1, 0);
        var newEntity = new Entity(1, 22);
        
        mockEntityAllocationDatabase.GetEntity(staleEntity.Id).Returns(newEntity);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        var isValid = entityComponentAccessor.IsEntityValid(staleEntity);
        
        mockEntityAllocationDatabase.Received(1).GetEntity(staleEntity.Id);

        Assert.False(isValid);
    }
        
    [Fact]
    public void should_return_false_when_validating_entity_with_which_doesnt_exist_anymore()
    {
        var mockComponentDatabase = Substitute.For<IComponentDatabase>();
        var mockEntityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
        var mockEntityChangeRouter = Substitute.For<IEntityChangeRouter>();
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();

        var staleEntity = new Entity(1, 0);
        
        mockEntityAllocationDatabase.GetEntity(staleEntity.Id).Returns((Entity?)null);
        
        var entityComponentAccessor = new EntityComponentAccessor(mockComponentTypeLookup, mockEntityAllocationDatabase, 
            mockComponentDatabase, mockEntityChangeRouter);

        var isValid = entityComponentAccessor.IsEntityValid(staleEntity);
        
        mockEntityAllocationDatabase.Received(1).GetEntity(staleEntity.Id);

        Assert.False(isValid);
    }
}
