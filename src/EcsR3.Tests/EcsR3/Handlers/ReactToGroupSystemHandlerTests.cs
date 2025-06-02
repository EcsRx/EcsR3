using System;
using System.Collections.Generic;
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
            
            mockSystem.ReceivedWithAnyArgs(2).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Any<Entity>());
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_execute_system_without_predicate_with_pre_post_augments()
        {
            var entity1 = new Entity(1,0);
            var entity2 = new Entity(2, 0);
            var fakeEntities = new List<Entity> { entity1 ,entity2 };
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.Count.Returns(fakeEntities.Count);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var threadHandler = Substitute.For<IThreadHandler>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var observableSubject = new Subject<IComputedEntityGroup>();
            var mockSystem = Substitute.For<IReactToGroupSystem, ISystemPreProcessor, ISystemPostProcessor>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockComputedEntityGroup)).Returns(observableSubject);

            var mockPreProcessor = mockSystem as ISystemPreProcessor;
            var mockPostProcessor = mockSystem as ISystemPostProcessor;
            
            var systemHandler = new ReactToGroupSystemHandler(entityComponentAccessor, observableGroupManager, threadHandler);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockComputedEntityGroup);
            
            mockSystem.ReceivedWithAnyArgs(2).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Any<Entity>());
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