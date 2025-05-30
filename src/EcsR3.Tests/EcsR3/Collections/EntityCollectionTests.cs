using EcsR3.Collections.Entities;
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
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var entityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
            entityAllocationDatabase.AllocateEntity().Returns(1);

            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            
            var wasCalled = false;
            entityCollection.OnAdded.Subscribe(x => wasCalled = true);
            
            var entity = entityCollection.Create();
            
            Assert.Contains(1, entityCollection.EntityLookup);
            Assert.Equal(1, entity);
            Assert.True(wasCalled);
        }

        [Fact]
        public void should_raise_events_and_remove_components_when_removing_entity()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var entityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
            var id1 = 1;
            
            entityAllocationDatabase.AllocateEntity().Returns(id1);
           
            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            
            var wasCalled = false;
            entityCollection.OnRemoved.Subscribe(x => wasCalled = true);
            
            entityCollection.Create();
            entityCollection.Remove(id1);

            Assert.True(wasCalled);
            Assert.DoesNotContain(id1, entityCollection);
        }
        
        [Fact]
        public void should_remove_components_from_entity_when_removing_from_collection()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var entityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
            var id1 = 1;
            
            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            entityCollection.EntityLookup.Add(id1);
            
            entityCollection.Remove(id1);

            entityComponentAccessor.Received(1).RemoveAllComponents(id1);
            Assert.DoesNotContain(id1, entityCollection);
        }
        
        [Fact]
        public void should_remove_components_from_all_entity_when_removing_all_from_collection()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var entityAllocationDatabase = Substitute.For<IEntityAllocationDatabase>();
            var id1 = 1;
            var id2 = 2;
            var id3 = 3;
           
            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            entityCollection.EntityLookup.Add(id1);
            entityCollection.EntityLookup.Add(id2);
            entityCollection.EntityLookup.Add(id3);
            
            entityCollection.RemoveAll();

            entityComponentAccessor.Received(1).RemoveAllComponents(id1);
            entityComponentAccessor.Received(1).RemoveAllComponents(id2);
            entityComponentAccessor.Received(1).RemoveAllComponents(id3);
            Assert.Empty(entityCollection);
        }
    }
}