using System;
using System.Collections.Generic;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Handlers;
using EcsR3.Systems.Reactive;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsR3.Handlers
{
    public class ReactToEntitySystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var reactToEntitySystemHandler = new ReactToEntitySystemHandler(entityComponentAccessor, observableGroupManager);
            
            var fakeMatchingSystem = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem1 = Substitute.For<ISetupSystem>();
            var fakeNonMatchingSystem2 = Substitute.For<IGroupSystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_execute_system_without_predicate()
        {
            var entity1 = new Entity(1,0);
            var entity2 = new Entity(2, 0);
            var fakeEntities = new List<Entity> { entity1 ,entity2 };
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.OnAdded.Returns(new Subject<Entity>());
            mockComputedEntityGroup.OnRemoved.Returns(new Subject<Entity>());
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var firstEntitySubject = new Subject<Entity>();
            var secondEntitySubject = new Subject<Entity>();
            var mockSystem = Substitute.For<IReactToEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1)).Returns(firstEntitySubject);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToEntitySystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            firstEntitySubject.OnNext(entity1);
            secondEntitySubject.OnNext(entity2);
            
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1));
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2));
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(entity1.Id));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(entity2.Id));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_execute_system_when_entity_added_to_group()
        {
            var entity1 = new Entity(1,0);
            var entity2 = new Entity(2, 0);
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(new List<Entity>().GetEnumerator());
            mockComputedEntityGroup.OnRemoved.Returns(new Subject<Entity>());

            var addedSubject = new Subject<Entity>();
            mockComputedEntityGroup.OnAdded.Returns(addedSubject);
            mockComputedEntityGroup.Contains(Arg.Is(entity1)).Returns(true);
            mockComputedEntityGroup.Contains(Arg.Is(entity2)).Returns(true);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var firstEntitySubject = new Subject<Entity>();
            var secondEntitySubject = new Subject<Entity>();
            var mockSystem = Substitute.For<IReactToEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1)).Returns(firstEntitySubject);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToEntitySystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);

            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(0, systemHandler._entitySubscriptions[mockSystem].Count);
            
            mockSystem.Received(0).ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Any<Entity>());
            addedSubject.OnNext(entity1);
            addedSubject.OnNext(entity2);

            mockSystem.Received(1).ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1));
            mockSystem.Received(1).ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2));
            
            firstEntitySubject.OnNext(entity1);
            secondEntitySubject.OnNext(entity2);
            
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1));
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2));
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(entity1.Id));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(entity2.Id));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_dispose_entity_subscriptions_when_removed_from_group()
        {
            var entity1 = new Entity(1,0);
            var entity2 = new Entity(2, 0);
            var fakeEntities = new List<Entity> { entity1 ,entity2 };
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.OnAdded.Returns(new Subject<Entity>());
            
            var removedSubject = new Subject<Entity>();
            mockComputedEntityGroup.OnRemoved.Returns(removedSubject);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var firstEntitySubject = new Subject<Entity>();
            var secondEntitySubject = new Subject<Entity>();
            var mockSystem = Substitute.For<IReactToEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1)).Returns(firstEntitySubject);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToEntitySystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(entity1.Id));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(entity2.Id));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);

            removedSubject.OnNext(entity1);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(1, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(entity2.Id));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
       
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var entity1 = new Entity(1,0);
            var entity2 = new Entity(2, 0);
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            var mockSystemDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactToEntitySystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler._systemSubscriptions.Add(mockSystem, mockSystemDisposable);
            
            var entitySubscriptions = new Dictionary<int, IDisposable>();
            var mockEntityDisposable1 = Substitute.For<IDisposable>();
            entitySubscriptions.Add(entity1.Id, mockEntityDisposable1);
            var mockEntityDisposable2 = Substitute.For<IDisposable>();
            entitySubscriptions.Add(entity2.Id, mockEntityDisposable2);
            systemHandler._entitySubscriptions.Add(mockSystem, entitySubscriptions);
            
            systemHandler.DestroySystem(mockSystem);
            
            mockSystemDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
            
            mockEntityDisposable1.Received(1).Dispose();
            mockEntityDisposable2.Received(1).Dispose();
            Assert.Equal(0, systemHandler._entitySubscriptions.Count);
        }
    }
}