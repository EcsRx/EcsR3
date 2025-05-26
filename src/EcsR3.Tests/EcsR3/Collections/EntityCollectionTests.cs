using EcsR3.Collections.Entity;
using EcsR3.Entities;
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
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            var mockEntity = Substitute.For<IEntity>();
            mockEntityFactory.Create(null).Returns(mockEntity);

            var entityCollection = new EntityCollection(mockEntityFactory);
            
            var wasCalled = false;
            entityCollection.OnAdded.Subscribe(x => wasCalled = true);
            
            var entity = entityCollection.Create();
            
            Assert.Contains(mockEntity, entityCollection.EntityLookup.Values);
            Assert.Equal(mockEntity, entity);
            Assert.True(wasCalled);
        }

        [Fact]
        public void should_raise_events_and_remove_components_when_removing_entity()
        {
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            var mockEntity = Substitute.For<IEntity>();
            mockEntity.Id.Returns(1);
            
            mockEntityFactory.Create(null).Returns(mockEntity);
           
            var entityCollection = new EntityCollection(mockEntityFactory);
            
            var wasCalled = false;
            entityCollection.OnRemoved.Subscribe(x => wasCalled = true);
            
            entityCollection.Create();
            entityCollection.Remove(mockEntity.Id);

            Assert.True(wasCalled);
            Assert.DoesNotContain(mockEntity, entityCollection);
        }
        
        [Fact]
        public void should_remove_components_from_entity_when_removing_from_collection()
        {
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            var mockEntity = Substitute.For<IEntity>();
            mockEntity.Id.Returns(1);
            
            var entityCollection = new EntityCollection(mockEntityFactory);
            entityCollection.EntityLookup.Add(1, mockEntity);
            
            entityCollection.Remove(mockEntity.Id);

            mockEntity.Received(1).RemoveAllComponents();
            Assert.DoesNotContain(mockEntity, entityCollection);
        }
        
        [Fact]
        public void should_remove_components_from_all_entity_when_removing_all_from_collection()
        {
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            var mockEntity1 = Substitute.For<IEntity>();
            mockEntity1.Id.Returns(1);
            var mockEntity2 = Substitute.For<IEntity>();
            mockEntity2.Id.Returns(2);
            var mockEntity3 = Substitute.For<IEntity>();
            mockEntity3.Id.Returns(3);
            
           
            var entityCollection = new EntityCollection(mockEntityFactory);
            entityCollection.EntityLookup.Add(1, mockEntity1);
            entityCollection.EntityLookup.Add(2, mockEntity2);
            entityCollection.EntityLookup.Add(3, mockEntity3);
            
            entityCollection.RemoveAll();

            mockEntity1.Received(1).RemoveAllComponents();
            mockEntity2.Received(1).RemoveAllComponents();
            mockEntity3.Received(1).RemoveAllComponents();
            Assert.Empty(entityCollection);
        }
    }
}