using System;
using System.Collections.Generic;
using SystemsR3.Scheduling;
using SystemsR3.Systems.Conventional;
using SystemsR3.Threading;
using EcsR3.Collections;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using EcsR3.Systems;
using EcsR3.Systems.Handlers;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsRx.Handlers
{
    public class BasicEntitySystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var reactToEntitySystemHandler = new BasicEntitySystemHandler(observableGroupManager, threadHandler, observableScheduler);
            
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
            var fakeEntities = new List<IEntity>
            {
                Substitute.For<IEntity>(),
                Substitute.For<IEntity>()
            };
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup[0].Returns(fakeEntities[0]);
            mockObservableGroup[1].Returns(fakeEntities[1]);
            mockObservableGroup.Count.Returns(fakeEntities.Count);
            
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var observableSubject = new Subject<ElapsedTime>();
            observableScheduler.OnUpdate.Returns(observableSubject);
            
            var fakeGroup = Group.Empty;
            observableGroupManager.GetObservableGroup(Arg.Is(fakeGroup), Arg.Any<int[]>()).Returns(mockObservableGroup);

            var mockSystem = Substitute.For<IBasicEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            
            var systemHandler = new BasicEntitySystemHandler(observableGroupManager, threadHandler, observableScheduler);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(new ElapsedTime());
            
            mockSystem.ReceivedWithAnyArgs(2).Process(Arg.Any<IEntity>(), default);
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_only_execute_system_when_predicate_met()
        {
            var entityToMatch = Substitute.For<IEntity>();
            var idToMatch = 1;
            entityToMatch.Id.Returns(idToMatch);
            
            var fakeEntities = new List<IEntity>
            {
                entityToMatch,
                Substitute.For<IEntity>()
            };

            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockObservableGroup[0].Returns(fakeEntities[0]);
            mockObservableGroup[1].Returns(fakeEntities[1]);
            mockObservableGroup.Count.Returns(fakeEntities.Count);
            
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var observableSubject = new Subject<ElapsedTime>();
            observableScheduler.OnUpdate.Returns(observableSubject);
            
            var fakeGroup = new GroupWithPredicate(x => x.Id == idToMatch);
            observableGroupManager.GetObservableGroup(Arg.Is(fakeGroup), Arg.Any<int[]>()).Returns(mockObservableGroup);

            var mockSystem = Substitute.For<IBasicEntitySystem>();
            mockSystem.Group.Returns(fakeGroup);
            
            var systemHandler = new BasicEntitySystemHandler(observableGroupManager, threadHandler, observableScheduler);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(new ElapsedTime());
            
            mockSystem.ReceivedWithAnyArgs(1).Process(Arg.Is(entityToMatch), default);
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var mockSystem = Substitute.For<IBasicEntitySystem>();
            var mockDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new BasicEntitySystemHandler(observableGroupManager, threadHandler, observableScheduler);
            systemHandler._systemSubscriptions.Add(mockSystem, mockDisposable);
            systemHandler.DestroySystem(mockSystem);
            
            mockDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
        }
    }
}