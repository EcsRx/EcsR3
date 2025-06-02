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
    public class ReactToDataSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var computedEntityGroupRegistry = Substitute.For<IComputedEntityGroupRegistry>();
            var reactToEntitySystemHandler = new ReactToDataSystemHandler(entityComponentAccessor, computedEntityGroupRegistry);
            
            var fakeMatchingSystem1 = Substitute.For<IReactToDataSystem<int>>();
            var fakeMatchingSystem2 = Substitute.For<IReactToDataSystem<DateTime>>();
            var fakeNonMatchingSystem1 = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem2 = Substitute.For<IGroupSystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem1));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem2));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_execute_system_without_predicate()
        {
            var entity1 = new Entity(1,0);
            var entity2 = new Entity(2, 0);
            var fakeEntities = new List<Entity> { entity1 ,entity2 };
            
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.OnAdded.Returns(new Subject<Entity>());
            mockComputedEntityGroup.OnRemoved.Returns(new Subject<Entity>());
            
            var computedEntityGroupRegistry = Substitute.For<IComputedEntityGroupRegistry>();
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();

            var fakeGroup = Group.Empty;
            computedEntityGroupRegistry.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var firstEntitySubject = new Subject<int>();
            var secondEntitySubject = new Subject<int>();
            var mockSystem = Substitute.For<IReactToDataSystem<int>>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToData(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1)).Returns(firstEntitySubject);
            mockSystem.ReactToData(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToDataSystemHandler(entityComponentAccessor, computedEntityGroupRegistry);
            systemHandler.SetupSystem(mockSystem);
            
            firstEntitySubject.OnNext(1);
            secondEntitySubject.OnNext(2);
            
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1), Arg.Is(1));
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2), Arg.Is(2));
            
            Assert.Equal(1, systemHandler.SystemSubscriptions.Count);
            Assert.NotNull(systemHandler.SystemSubscriptions[mockSystem]);
            
            Assert.Equal(1, systemHandler.EntitySubscriptions.Count);
            Assert.Equal(2, systemHandler.EntitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler.EntitySubscriptions[mockSystem].Keys.Contains(entity1.Id));
            Assert.True(systemHandler.EntitySubscriptions[mockSystem].Keys.Contains(entity2.Id));
            Assert.All(systemHandler.EntitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_execute_system_when_entity_added_to_group()
        {
            var entity1 = new Entity(1,0);
            var entity2 = new Entity(2, 0);
            var fakeEntities = new List<Entity> { entity1 ,entity2 };
            
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(new List<Entity>().GetEnumerator());
            mockComputedEntityGroup.OnRemoved.Returns(new Subject<Entity>());

            var addedSubject = new Subject<Entity>();
            mockComputedEntityGroup.OnAdded.Returns(addedSubject);
            mockComputedEntityGroup.Contains(Arg.Is(entity1)).Returns(true);
            mockComputedEntityGroup.Contains(Arg.Is(entity2)).Returns(true);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var firstEntitySubject = new Subject<int>();
            var secondEntitySubject = new Subject<int>();
            var mockSystem = Substitute.For<IReactToDataSystem<int>>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToData(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1)).Returns(firstEntitySubject);
            mockSystem.ReactToData(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToDataSystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);

            Assert.Equal(1, systemHandler.EntitySubscriptions.Count);
            Assert.Equal(0, systemHandler.EntitySubscriptions[mockSystem].Count);
            
            mockSystem.Received(0).ReactToData(Arg.Any<IEntityComponentAccessor>(), Arg.Any<Entity>());
            addedSubject.OnNext(entity1);
            addedSubject.OnNext(entity2);

            mockSystem.Received(1).ReactToData(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1));
            mockSystem.Received(1).ReactToData(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2));
            
            firstEntitySubject.OnNext(1);
            secondEntitySubject.OnNext(2);
            
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1), Arg.Is(1));
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2), Arg.Is(2));
            
            Assert.Equal(1, systemHandler.SystemSubscriptions.Count);
            Assert.NotNull(systemHandler.SystemSubscriptions[mockSystem]);
            
            Assert.Equal(1, systemHandler.EntitySubscriptions.Count);
            Assert.Equal(2, systemHandler.EntitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler.EntitySubscriptions[mockSystem].Keys.Contains(entity1.Id));
            Assert.True(systemHandler.EntitySubscriptions[mockSystem].Keys.Contains(entity2.Id));
            Assert.All(systemHandler.EntitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_dispose_entity_subscriptions_when_removed_from_group()
        {
            var entity1 = new Entity(1,0);
            var entity2 = new Entity(2, 0);
            var fakeEntities = new List<Entity> { entity1 ,entity2 };
            
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.OnAdded.Returns(new Subject<Entity>());
            
            var removedSubject = new Subject<Entity>();
            mockComputedEntityGroup.OnRemoved.Returns(removedSubject);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var firstEntitySubject = new Subject<int>();
            var secondEntitySubject = new Subject<int>();
            var mockSystem = Substitute.For<IReactToDataSystem<int>>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToData(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1)).Returns(firstEntitySubject);
            mockSystem.ReactToData(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToDataSystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            Assert.Equal(1, systemHandler.EntitySubscriptions.Count);
            Assert.Equal(2, systemHandler.EntitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler.EntitySubscriptions[mockSystem].Keys.Contains(entity1.Id));
            Assert.True(systemHandler.EntitySubscriptions[mockSystem].Keys.Contains(entity2.Id));
            Assert.All(systemHandler.EntitySubscriptions[mockSystem].Values, Assert.NotNull);

            removedSubject.OnNext(entity1);
            
            Assert.Equal(1, systemHandler.EntitySubscriptions.Count);
            Assert.Equal(1, systemHandler.EntitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler.EntitySubscriptions[mockSystem].Keys.Contains(entity2.Id));
            Assert.All(systemHandler.EntitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var entity1 = new Entity(1,0);
            var entity2 = new Entity(2, 0);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            
            var mockSystem = Substitute.For<IReactToDataSystem<int>>();
            var mockSystemDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactToDataSystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SystemSubscriptions.Add(mockSystem, mockSystemDisposable);
            
            var entitySubscriptions = new Dictionary<int, IDisposable>();
            var mockEntityDisposable1 = Substitute.For<IDisposable>();
            entitySubscriptions.Add(entity1.Id, mockEntityDisposable1);
            var mockEntityDisposable2 = Substitute.For<IDisposable>();
            entitySubscriptions.Add(entity2.Id, mockEntityDisposable2);
            systemHandler.EntitySubscriptions.Add(mockSystem, entitySubscriptions);
            
            systemHandler.DestroySystem(mockSystem);
            
            mockSystemDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler.SystemSubscriptions.Count);
            
            mockEntityDisposable1.Received(1).Dispose();
            mockEntityDisposable2.Received(1).Dispose();
            Assert.Equal(0, systemHandler.EntitySubscriptions.Count);
        }
        
        public interface IReactToMultipleDataSystem : IReactToDataSystem<string>, IReactToDataSystem<int> { }
        
        [Fact]
        public void should_execute_system_with_multiple_interfaces_when_entity_added_to_group()
        {
            var entity1 = new Entity(1,0);
            var fakeEntities = new List<Entity> { entity1 };

            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.OnAdded.Returns(new Subject<Entity>());
            mockComputedEntityGroup.OnRemoved.Returns(new Subject<Entity>());

            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var firstEntitySubjectString = new Subject<string>();
            var firstEntitySubjectInt = new Subject<int>();
            var mockSystem = Substitute.For<IReactToMultipleDataSystem>();
            mockSystem.Group.Returns(fakeGroup);
            ((IReactToDataSystem<string>)mockSystem).ReactToData(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1)).Returns(firstEntitySubjectString);
            ((IReactToDataSystem<int>)mockSystem).ReactToData(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1)).Returns(firstEntitySubjectInt);

            var systemHandler = new ReactToDataSystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);

            firstEntitySubjectString.OnNext("One");
            firstEntitySubjectInt.OnNext(1);

            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1), Arg.Is("One"));
            mockSystem.Received(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Is(entity1), Arg.Is(1));
        }
    }
}