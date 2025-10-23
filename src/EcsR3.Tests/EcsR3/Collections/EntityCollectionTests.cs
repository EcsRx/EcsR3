using EcsR3.Collections.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsR3.Collections
{
    public class EntityCollectionTests
    {
        [Fact]
        public void should_create_new_entity_and_raise_event()
        {
            var expectedEntity = new Entity(1, 0);
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var entityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
            entityAllocationDatabase.AllocateEntity().Returns(expectedEntity);

            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            
            var wasCalled = false;
            entityCollection.OnAdded.Subscribe(x => wasCalled = true);
            
            var entity = entityCollection.Create();
            
            Assert.Contains(entity, entityCollection.EntityLookup);
            Assert.Equal(expectedEntity, entity);
            Assert.True(wasCalled);
        }

        [Fact]
        public void should_raise_events_and_remove_components_when_removing_entity()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var entityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
            var entity = new Entity(1, 0);
            
            entityAllocationDatabase.AllocateEntity().Returns(entity);
           
            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            
            var wasCalled = false;
            entityCollection.OnRemoved.Subscribe(x => wasCalled = true);
            
            entityCollection.Create();
            entityCollection.Remove(entity);

            Assert.True(wasCalled);
            Assert.DoesNotContain(entity, entityCollection);
        }
        
        [Fact]
        public void should_remove_components_from_entity_when_removing_from_collection()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var entityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
            var entity = new Entity(1, 0);
            
            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            entityCollection.EntityLookup.Add(entity);
            
            entityCollection.Remove(entity);

            entityComponentAccessor.Received(1).RemoveAllComponents(entity);
            Assert.DoesNotContain(entity, entityCollection);
        }
        
        [Fact]
        public void should_remove_components_from_all_entity_when_removing_all_from_collection()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var entityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
            var entity1 = new Entity(1, 0);
            var entity2 = new Entity(2, 0);
            var entity3 = new Entity(3, 0);
            
            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            entityCollection.EntityLookup.Add(entity1);
            entityCollection.EntityLookup.Add(entity2);
            entityCollection.EntityLookup.Add(entity3);
            
            entityCollection.Clear();

            entityComponentAccessor.Received(1).RemoveAllComponents(entity1);
            entityComponentAccessor.Received(1).RemoveAllComponents(entity2);
            entityComponentAccessor.Received(1).RemoveAllComponents(entity3);
            Assert.Empty(entityCollection);
        }
        
        [Fact]
        public void should_return_true_for_contains_with_valid_entity()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var entityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
            var entity1 = new Entity(1, 0);
            
            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            entityCollection.EntityLookup.Add(entity1);
            
            var contains = entityCollection.Contains(entity1);
            
            Assert.True(contains);
        }
        
        [Fact]
        public void should_return_false_for_contains_with_stale_entity()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var entityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
            var staleEntity1 = new Entity(1, 0);
            var currentEntity1 = new Entity(1, 22);
            
            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            entityCollection.EntityLookup.Add(currentEntity1);
            
            var contains = entityCollection.Contains(staleEntity1);
            
            Assert.False(contains);
        }
        
                
        [Fact]
        public void should_dispose_correctly_and_remove_all_entities()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var entityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
            var entity1 = new Entity(1, 0);
            var entity2 = new Entity(2, 0);
            
            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            entityCollection.EntityLookup.Add(entity1);
            entityCollection.EntityLookup.Add(entity2);

            entityCollection.Dispose();
            
            Assert.Empty(entityCollection);
            
            entityComponentAccessor.Received(1).RemoveAllComponents(entity1);
            entityComponentAccessor.Received(1).RemoveAllComponents(entity2);
        }
    }
}