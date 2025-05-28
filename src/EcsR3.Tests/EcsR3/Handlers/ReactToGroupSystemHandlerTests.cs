using System;
using System.Collections.Generic;
using System.Linq;
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
using SystemsR3.Threading;
using Xunit;

namespace EcsR3.Tests.EcsR3.Handlers
{
    public class ReactToGroupSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var reactToEntitySystemHandler = new ReactToGroupSystemHandler(entityComponentAccessor, observableGroupManager, threadHandler);
            
            var fakeMatchingSystem = Substitute.For<IReactToGroupSystem>();
            var fakeMatchingSystem2 = Substitute.For<IReactToGroupExSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<ISetupSystem>();
            var fakeNonMatchingSystem2 = Substitute.For<IGroupSystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem2));
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
            mockComputedEntityGroup.Count.Returns(fakeEntities.Count);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var threadHandler = Substitute.For<IThreadHandler>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var observableSubject = new Subject<IComputedEntityGroup>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockComputedEntityGroup)).Returns(observableSubject);
            
            var systemHandler = new ReactToGroupSystemHandler(entityComponentAccessor, observableGroupManager, threadHandler);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockComputedEntityGroup);
            
            mockSystem.ReceivedWithAnyArgs(2).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Any<int>());
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_execute_system_without_predicate_with_pre_post()
        {
            var id1 = 1;
            var id2 = 2;
            var fakeEntities = new List<int> { id1, id2 };
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.Count.Returns(fakeEntities.Count);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var threadHandler = Substitute.For<IThreadHandler>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var observableSubject = new Subject<IComputedEntityGroup>();
            var mockSystem = Substitute.For<IReactToGroupExSystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockComputedEntityGroup)).Returns(observableSubject);
            
            var systemHandler = new ReactToGroupSystemHandler(entityComponentAccessor, observableGroupManager, threadHandler);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockComputedEntityGroup);
            
            mockSystem.ReceivedWithAnyArgs(2).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Any<int>());
            mockSystem.ReceivedWithAnyArgs(1).BeforeProcessing();
            mockSystem.ReceivedWithAnyArgs(1).AfterProcessing();
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
       
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            var mockDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactToGroupSystemHandler(entityComponentAccessor, observableGroupManager, threadHandler);
            systemHandler._systemSubscriptions.Add(mockSystem, mockDisposable);
            systemHandler.DestroySystem(mockSystem);
            
            mockDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
        }
    }
}