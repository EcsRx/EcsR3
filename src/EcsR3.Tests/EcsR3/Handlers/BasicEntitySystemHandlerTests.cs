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
using SystemsR3.Scheduling;
using SystemsR3.Systems.Conventional;
using SystemsR3.Threading;
using Xunit;

namespace EcsR3.Tests.EcsR3.Handlers
{
    public class BasicEntitySystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var reactToEntitySystemHandler = new BasicEntitySystemHandler(entityComponentAccessor, observableGroupManager, threadHandler, observableScheduler);
            
            var fakeMatchingSystem = Substitute.For<IBasicEntitySystem>();
            var fakeNonMatchingSystem1 = Substitute.For<IManualSystem>();
            var fakeNonMatchingSystem2 = Substitute.For<IGroupSystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_execute_system_without_predicate()
        {
            var fakeEntities = new List<int> { 1,2 };
            
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockComputedEntityGroup.Count.Returns(fakeEntities.Count);
            
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var observableSubject = new Subject<ElapsedTime>();
            observableScheduler.OnUpdate.Returns(observableSubject);
            
            var fakeGroup = Group.Empty;
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);

            var mockSystem = Substitute.For<IBasicEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            
            var systemHandler = new BasicEntitySystemHandler(entityComponentAccessor, observableGroupManager, threadHandler, observableScheduler);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(new ElapsedTime());
            
            mockSystem.ReceivedWithAnyArgs(2).Process(Arg.Any<IEntityComponentAccessor>(), Arg.Any<int>(), default);
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var mockSystem = Substitute.For<IBasicEntitySystem>();
            var mockDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new BasicEntitySystemHandler(entityComponentAccessor, observableGroupManager, threadHandler, observableScheduler);
            systemHandler._systemSubscriptions.Add(mockSystem, mockDisposable);
            systemHandler.DestroySystem(mockSystem);
            
            mockDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
        }
    }
}