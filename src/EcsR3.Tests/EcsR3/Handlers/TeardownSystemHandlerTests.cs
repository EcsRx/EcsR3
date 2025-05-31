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
    public class TeardownSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        { 
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var teardownSystemHandler = new TeardownSystemHandler(entityComponentAccessor, observableGroupManager);
            
            var fakeMatchingSystem = Substitute.For<ITeardownSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem2 = Substitute.For<IGroupSystem>();
            
            Assert.True(teardownSystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(teardownSystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(teardownSystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_teardown_entity_when_removed()
        {
            var id1 = 1;
            var fakeEntities = new List<int> { id1 };

            var removeSubject = new Subject<int>();
            var mockComputedEntityGroup = Substitute.For<IComputedEntityGroup>();
            mockComputedEntityGroup.OnAdded.Returns(new Subject<int>());
            mockComputedEntityGroup.OnRemoving.Returns(removeSubject);
            mockComputedEntityGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            
            var entityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();

            var fakeGroup = Substitute.For<IGroup>();
            fakeGroup.RequiredComponents.Returns(new Type[0]);
            observableGroupManager.GetComputedGroup(Arg.Is(fakeGroup)).Returns(mockComputedEntityGroup);
            
            var mockSystem = Substitute.For<ITeardownSystem>();
            mockSystem.Group.Returns(fakeGroup);

            var systemHandler = new TeardownSystemHandler(entityComponentAccessor, observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            removeSubject.OnNext(id1);
            
            mockSystem.Received(1).Teardown(Arg.Any<IEntityComponentAccessor>(), Arg.Is(id1));
            Assert.Equal(1, systemHandler.SystemSubscriptions.Count);
            Assert.NotNull(systemHandler.SystemSubscriptions[mockSystem]);
        }
    }
}