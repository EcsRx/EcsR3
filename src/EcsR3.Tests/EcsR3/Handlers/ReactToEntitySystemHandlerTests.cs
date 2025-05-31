using System;
using System.Collections.Generic;
using EcsR3.Collections;
using EcsR3.Computeds;
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
            var id1 = 1;
            var id2 = 2;
            var fakeEntities = new List<int> { id1, id2 };
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.OnAdded.Returns(new Subject<int>());
            mockComputedEntityGroup.OnRemoved.Returns(new Subject<int>());
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var firstEntitySubject = new Subject<int>();
            var secondEntitySubject = new Subject<int>();
            var mockSystem = Substitute.For<IReactToEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id1)).Returns(firstEntitySubject);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToEntitySystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            firstEntitySubject.OnNext(id1);
            secondEntitySubject.OnNext(id2);
            
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id1));
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id2));
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id1));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_execute_system_when_entity_added_to_group()
        {
            var id1 = 1;
            var id2 = 2;
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(new List<int>().GetEnumerator());
            mockComputedEntityGroup.OnRemoved.Returns(new Subject<int>());

            var addedSubject = new Subject<int>();
            mockComputedEntityGroup.OnAdded.Returns(addedSubject);
            mockComputedEntityGroup.Contains(Arg.Is(id1)).Returns(true);
            mockComputedEntityGroup.Contains(Arg.Is(id2)).Returns(true);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var firstEntitySubject = new Subject<int>();
            var secondEntitySubject = new Subject<int>();
            var mockSystem = Substitute.For<IReactToEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id1)).Returns(firstEntitySubject);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToEntitySystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);

            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(0, systemHandler._entitySubscriptions[mockSystem].Count);
            
            mockSystem.Received(0).ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Any<int>());
            addedSubject.OnNext(id1);
            addedSubject.OnNext(id2);

            mockSystem.Received(1).ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id1));
            mockSystem.Received(1).ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id2));
            
            firstEntitySubject.OnNext(id1);
            secondEntitySubject.OnNext(id2);
            
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id1));
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id2));
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id1));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_dispose_entity_subscriptions_when_removed_from_group()
        {
            var id1 = 1;
            var id2 = 2;
            var fakeEntities = new List<int> { id1, id2 };
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.OnAdded.Returns(new Subject<int>());
            
            var removedSubject = new Subject<int>();
            mockComputedEntityGroup.OnRemoved.Returns(removedSubject);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var firstEntitySubject = new Subject<int>();
            var secondEntitySubject = new Subject<int>();
            var mockSystem = Substitute.For<IReactToEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id1)).Returns(firstEntitySubject);
            mockSystem.ReactToEntity(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToEntitySystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id1));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);

            removedSubject.OnNext(id1);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(1, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
       
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var id1 = 1;
            var id2 = 2;
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            var mockSystemDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactToEntitySystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler._systemSubscriptions.Add(mockSystem, mockSystemDisposable);
            
            var entitySubscriptions = new Dictionary<int, IDisposable>();
            var mockEntityDisposable1 = Substitute.For<IDisposable>();
            entitySubscriptions.Add(id1, mockEntityDisposable1);
            var mockEntityDisposable2 = Substitute.For<IDisposable>();
            entitySubscriptions.Add(id2, mockEntityDisposable2);
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