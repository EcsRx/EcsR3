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
using EcsR3.Systems.Augments;
using EcsR3.Systems.Handlers;
using EcsR3.Systems.Reactive;
using NSubstitute;
using R3;
using SystemsR3.Threading;
using Xunit;

namespace EcsR3.Tests.EcsR3.Handlers
{
    public class ReactToGroupBatchedSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var systemHandler = new ReactToGroupBatchedSystemHandler(entityComponentAccessor, observableGroupManager);
            
            var fakeMatchingSystem = Substitute.For<IReactToGroupBatchedSystem>();
            var fakeNonMatchingSystem = Substitute.For<IReactToGroupSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<ISetupSystem>();
            var fakeNonMatchingSystem2 = Substitute.For<IGroupSystem>();
            
            Assert.True(systemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(systemHandler.CanHandleSystem(fakeNonMatchingSystem));
            Assert.False(systemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(systemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_execute_system_without()
        {
            var id1 = 1;
            var id2 = 2;
            var fakeEntities = new List<int> { id1, id2 };
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.Count.Returns(fakeEntities.Count);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var observableSubject = new Subject<IComputedEntityGroup>();
            var mockSystem = Substitute.For<IReactToGroupBatchedSystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockComputedEntityGroup)).Returns(observableSubject);
            
            var systemHandler = new ReactToGroupBatchedSystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockComputedEntityGroup);
            
            mockSystem.ReceivedWithAnyArgs(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Any<IReadOnlyList<int>>());
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_execute_system_with_pre_post_augments()
        {
            var id1 = 1;
            var id2 = 2;
            var fakeEntities = new List<int> { id1, id2 };
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.Count.Returns(fakeEntities.Count);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var observableSubject = new Subject<IComputedEntityGroup>();
            var mockSystem = Substitute.For<IReactToGroupBatchedSystem, ISystemPreProcessor, ISystemPostProcessor>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockComputedEntityGroup)).Returns(observableSubject);

            var mockPreProcessor = mockSystem as ISystemPreProcessor;
            var mockPostProcessor = mockSystem as ISystemPostProcessor;
            
            var systemHandler = new ReactToGroupBatchedSystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockComputedEntityGroup);
            
            mockSystem.ReceivedWithAnyArgs(1).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Any<IReadOnlyList<int>>());
            mockPreProcessor.ReceivedWithAnyArgs(1).BeforeProcessing();
            mockPostProcessor.ReceivedWithAnyArgs(1).AfterProcessing();
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
       
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var mockSystem = Substitute.For<IReactToGroupBatchedSystem>();
            var mockDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactToGroupBatchedSystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler._systemSubscriptions.Add(mockSystem, mockDisposable);
            systemHandler.DestroySystem(mockSystem);
            
            mockDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
        }
    }
}