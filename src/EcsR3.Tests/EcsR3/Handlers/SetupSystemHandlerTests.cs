using System;
using System.Collections.Generic;
using System.Threading;
using EcsR3.Collections;
using EcsR3.Computeds;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Handlers;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsR3.Handlers
{
    public class SetupSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var computedEntityGroupRegistry = Substitute.For<IComputedEntityGroupRegistry>();
            var reactToEntitySystemHandler = new SetupSystemHandler(entityComponentAccessor, computedEntityGroupRegistry);
            
            var fakeMatchingSystem = Substitute.For<ISetupSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem2 = Substitute.For<IGroupSystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_execute_system()
        {
            var id1 = 1;
            var id2 = 2;
            var fakeEntities = new List<int> { id1, id2 };
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.OnAdded.Returns(new Subject<int>());
            mockComputedEntityGroup.OnRemoved.Returns(new Subject<int>());
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Substitute.For<IGroup>();
            fakeGroup.RequiredComponents.Returns(Type.EmptyTypes);
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);
            
            var mockSystem = Substitute.For<ISetupSystem>();
            mockSystem.Group.Returns(fakeGroup);

            var systemHandler = new SetupSystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            mockSystem.Received(1).Setup(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id1));
            mockSystem.Received(1).Setup(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id2));
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
        }
        
        [Fact]
        public void should_execute_system_when_entity_added()
        {
            var id1 = 1;
            var id2 = 2;
            var fakeEntities = new List<int> { id1, id2 };
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            var addingSubject = new Subject<int>();
            mockComputedEntityGroup.OnAdded.Returns(addingSubject);
            mockComputedEntityGroup.Contains(Arg.Is(id1)).Returns(true);
            mockComputedEntityGroup.Contains(Arg.Is(id2)).Returns(true);
            mockComputedEntityGroup.OnRemoved.Returns(new Subject<int>());
            mockComputedEntityGroup.GetEnumerator().Returns(new List<int>().GetEnumerator());
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Substitute.For<IGroup>();
            fakeGroup.RequiredComponents.Returns(new Type[0]);
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);
            
            var mockSystem = Substitute.For<ISetupSystem>();
            mockSystem.Group.Returns(fakeGroup);

            var systemHandler = new SetupSystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            mockSystem.Received(0).Setup(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id1));
            mockSystem.Received(0).Setup(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id2));
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(0, systemHandler._entitySubscriptions[mockSystem].Count);

            addingSubject.OnNext(id1);
            addingSubject.OnNext(id2);
            
            mockSystem.Received(1).Setup(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id1));
            mockSystem.Received(1).Setup(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id2));
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
        }
        
        [Fact]
        public void should_dispose_observables_when_entity_removed()
        {
            var id1 = 1;
            var fakeEntities = new List<int>();
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            var removingSubject = new Subject<int>();
            mockComputedEntityGroup.OnAdded.Returns(new Subject<int>());
            mockComputedEntityGroup.OnRemoved.Returns(removingSubject);
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Substitute.For<IGroup>();
            fakeGroup.RequiredComponents.Returns(new Type[0]);
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);
            
            var mockSystem = Substitute.For<ISetupSystem>();
            mockSystem.Group.Returns(fakeGroup);

            var systemHandler = new SetupSystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);

            var mockDisposable = Substitute.For<IDisposable>();
            systemHandler._entitySubscriptions[mockSystem].Add(id1, mockDisposable);
            
            removingSubject.OnNext(id1);
            
            mockDisposable.Received(1).Dispose();
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(0, systemHandler._entitySubscriptions[mockSystem].Count);
        }
    }
}