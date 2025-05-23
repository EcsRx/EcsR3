using System;
using System.Collections.Generic;
using EcsR3.Collections;
using EcsR3.Computeds;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Handlers;
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
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var reactToEntitySystemHandler = new ReactToEntitySystemHandler(observableGroupManager);
            
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
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            var fakeEntities = new List<IEntity> { fakeEntity1, fakeEntity2 };

            var id1 = 1;
            var id2 = 2;
            fakeEntity1.Id.Returns(id1);
            fakeEntity2.Id.Returns(id2);
            
            var mockObservableGroup = Substitute.For<IComputedEntityGroup>();
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockObservableGroup.OnAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnRemoved.Returns(new Subject<IEntity>());
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var firstEntitySubject = new Subject<IEntity>();
            var secondEntitySubject = new Subject<IEntity>();
            var mockSystem = Substitute.For<IReactToEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToEntity(Arg.Is(fakeEntity1)).Returns(firstEntitySubject);
            mockSystem.ReactToEntity(Arg.Is(fakeEntity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToEntitySystemHandler(observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            firstEntitySubject.OnNext(fakeEntity1);
            secondEntitySubject.OnNext(fakeEntity2);
            
            mockSystem.Received(1).Process(Arg.Is(fakeEntity1));
            mockSystem.Received(1).Process(Arg.Is(fakeEntity2));
            
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
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();

            var id1 = 1;
            var id2 = 2;
            fakeEntity1.Id.Returns(id1);
            fakeEntity2.Id.Returns(id2);
            
            var mockObservableGroup = Substitute.For<IComputedEntityGroup>();
            mockObservableGroup.GetEnumerator().Returns(new List<IEntity>().GetEnumerator());
            mockObservableGroup.OnRemoved.Returns(new Subject<IEntity>());

            var addedSubject = new Subject<IEntity>();
            mockObservableGroup.OnAdded.Returns(addedSubject);
            mockObservableGroup.Contains(Arg.Is(fakeEntity1.Id)).Returns(true);
            mockObservableGroup.Contains(Arg.Is(fakeEntity2.Id)).Returns(true);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var firstEntitySubject = new Subject<IEntity>();
            var secondEntitySubject = new Subject<IEntity>();
            var mockSystem = Substitute.For<IReactToEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToEntity(Arg.Is(fakeEntity1)).Returns(firstEntitySubject);
            mockSystem.ReactToEntity(Arg.Is(fakeEntity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToEntitySystemHandler(observableGroupManager);
            systemHandler.SetupSystem(mockSystem);

            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(0, systemHandler._entitySubscriptions[mockSystem].Count);
            
            mockSystem.Received(0).ReactToEntity(Arg.Any<IEntity>());
            addedSubject.OnNext(fakeEntity1);
            addedSubject.OnNext(fakeEntity2);

            mockSystem.Received(1).ReactToEntity(Arg.Is(fakeEntity1));
            mockSystem.Received(1).ReactToEntity(Arg.Is(fakeEntity2));
            
            firstEntitySubject.OnNext(fakeEntity1);
            secondEntitySubject.OnNext(fakeEntity2);
            
            mockSystem.Received(1).Process(Arg.Is(fakeEntity1));
            mockSystem.Received(1).Process(Arg.Is(fakeEntity2));
            
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
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            var fakeEntities = new List<IEntity> { fakeEntity1, fakeEntity2 };

            var id1 = 1;
            var id2 = 2;
            fakeEntity1.Id.Returns(id1);
            fakeEntity2.Id.Returns(id2);
            
            var mockObservableGroup = Substitute.For<IComputedEntityGroup>();
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockObservableGroup.OnAdded.Returns(new Subject<IEntity>());
            
            var removedSubject = new Subject<IEntity>();
            mockObservableGroup.OnRemoved.Returns(removedSubject);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var firstEntitySubject = new Subject<IEntity>();
            var secondEntitySubject = new Subject<IEntity>();
            var mockSystem = Substitute.For<IReactToEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToEntity(Arg.Is(fakeEntity1)).Returns(firstEntitySubject);
            mockSystem.ReactToEntity(Arg.Is(fakeEntity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToEntitySystemHandler(observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id1));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);

            removedSubject.OnNext(fakeEntity1);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(1, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_only_execute_system_when_predicate_met()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            var fakeEntities = new List<IEntity> { fakeEntity1, fakeEntity2 };

            var id1 = 1;
            var id2 = 2;
            fakeEntity1.Id.Returns(id1);
            fakeEntity2.Id.Returns(id2);
            
            var mockObservableGroup = Substitute.For<IComputedEntityGroup>();
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockObservableGroup.OnAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnRemoved.Returns(new Subject<IEntity>());
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = new GroupWithPredicate(x => x.Id == id1);
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var firstEntitySubject = new Subject<IEntity>();
            var secondEntitySubject = new Subject<IEntity>();
            var mockSystem = Substitute.For<IReactToEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToEntity(Arg.Is(fakeEntity1)).Returns(firstEntitySubject);
            mockSystem.ReactToEntity(Arg.Is(fakeEntity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToEntitySystemHandler(observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            firstEntitySubject.OnNext(fakeEntity1);
            secondEntitySubject.OnNext(fakeEntity2);
            
            mockSystem.Received(1).Process(Arg.Is(fakeEntity1));
            mockSystem.Received(0).Process(Arg.Is(fakeEntity2));
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id1));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(id2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var id1 = 1;
            var id2 = 2;
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            var mockSystemDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactToEntitySystemHandler(observableGroupManager);
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