using System;
using System.Collections.Generic;
using EcsR3.Entities.Routing;
using EcsR3.Groups;
using EcsR3.Groups.Observable.Tracking.Trackers;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsRx.Observables.Trackers
{
    public class EntityRouterObservableGroupTrackerTests
    {
        [Fact]
        public void should_match_start_listening_for_required_changes_when_created()
        {
            // 1,2 are required, 3 is excluded
            var lookupGroup = new LookupGroup(new[] { 1,2 }, new [] {3});

            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            entityChangeRouter.OnEntityAddedComponent(Arg.Any<int>()).Returns(new Subject<EntityChange>());
            entityChangeRouter.OnEntityRemovingComponent(Arg.Any<int>()).Returns(new Subject<EntityChange>());
            entityChangeRouter.OnEntityRemovedComponent(Arg.Any<int>()).Returns(new Subject<EntityChange>());
            var groupTracker = new EntityRouterObservableGroupTracker(entityChangeRouter, lookupGroup);

            entityChangeRouter.Received(1).OnEntityAddedComponent(1);
            entityChangeRouter.Received(1).OnEntityAddedComponent(2);
            entityChangeRouter.Received(1).OnEntityAddedComponent(3);
            entityChangeRouter.Received(1).OnEntityRemovingComponent(1);
            entityChangeRouter.Received(1).OnEntityRemovingComponent(2);
            entityChangeRouter.Received(1).OnEntityRemovedComponent(1);
            entityChangeRouter.Received(1).OnEntityRemovedComponent(2);
            entityChangeRouter.Received(1).OnEntityRemovedComponent(3);
        }
        
        [Fact]
        public void should_raise_joined_event_when_entity_adds_needed_components()
        {
            // 1, 2 required
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            var addedComponent1Subject = new Subject<EntityChange>();
            var addedComponent2Subject = new Subject<EntityChange>();
            entityChangeRouter.OnEntityAddedComponent(1).Returns(addedComponent1Subject);
            entityChangeRouter.OnEntityAddedComponent(2).Returns(addedComponent2Subject);
            entityChangeRouter.OnEntityRemovingComponent(Arg.Any<int>()).Returns(new Subject<EntityChange>());
            entityChangeRouter.OnEntityRemovedComponent(Arg.Any<int>()).Returns(new Subject<EntityChange>());
            var groupTracker = new EntityRouterObservableGroupTracker(entityChangeRouter, lookupGroup);

            var invocations = new List<int>();
            groupTracker.OnEntityJoinedGroup.Subscribe(invocations.Add);
            
            addedComponent1Subject.OnNext(new EntityChange(50, 1));
            addedComponent2Subject.OnNext(new EntityChange(50, 2));
            addedComponent1Subject.OnNext(new EntityChange(12, 1));
            addedComponent2Subject.OnNext(new EntityChange(16, 2));
            addedComponent2Subject.OnNext(new EntityChange(15, 2));
            addedComponent1Subject.OnNext(new EntityChange(15, 1));
            
            Assert.NotEmpty(invocations);
            Assert.Equal(2, invocations.Count);
            Assert.Contains(50, invocations);
            Assert.Contains(15, invocations);
        }
        
        [Fact]
        public void should_raise_leaving_and_left_event_when_entity_removes_needed_components()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            var removingComponent1Subject = new Subject<EntityChange>();
            var removingComponent2Subject = new Subject<EntityChange>();
            var removedComponent1Subject = new Subject<EntityChange>();
            var removedComponent2Subject = new Subject<EntityChange>();
            entityChangeRouter.OnEntityAddedComponent(Arg.Any<int>()).Returns(new Subject<EntityChange>());
            entityChangeRouter.OnEntityRemovingComponent(1).Returns(removingComponent1Subject);
            entityChangeRouter.OnEntityRemovingComponent(2).Returns(removingComponent2Subject);
            entityChangeRouter.OnEntityRemovedComponent(1).Returns(removedComponent1Subject);
            entityChangeRouter.OnEntityRemovedComponent(2).Returns(removedComponent2Subject);
            var groupTracker = new EntityRouterObservableGroupTracker(entityChangeRouter, lookupGroup);

            var leavingInvocations = new List<int>();
            groupTracker.OnEntityLeavingGroup.Subscribe(leavingInvocations.Add);
            
            var leftInvocations = new List<int>();
            groupTracker.OnEntityLeftGroup.Subscribe(leftInvocations.Add);
            
            // 50 and 15 are presumed added, 12, 16 not yet
            groupTracker.EntityIdMatchState.Add(50, new GroupMatchingState(0, 0));
            groupTracker.EntityIdMatchState.Add(12, new GroupMatchingState(1, 0));
            groupTracker.EntityIdMatchState.Add(16, new GroupMatchingState(1, 0));
            groupTracker.EntityIdMatchState.Add(15, new GroupMatchingState(0, 0));
            
            removingComponent1Subject.OnNext(new EntityChange(50, 1));
            removedComponent1Subject.OnNext(new EntityChange(50, 1));
            removingComponent2Subject.OnNext(new EntityChange(50, 2));
            removedComponent2Subject.OnNext(new EntityChange(50, 2));
            removingComponent1Subject.OnNext(new EntityChange(12, 1));
            removedComponent1Subject.OnNext(new EntityChange(12, 1));
            removingComponent2Subject.OnNext(new EntityChange(16, 2));
            removedComponent2Subject.OnNext(new EntityChange(16, 2));
            removingComponent2Subject.OnNext(new EntityChange(15, 2));
            removedComponent2Subject.OnNext(new EntityChange(15, 2));
            removingComponent1Subject.OnNext(new EntityChange(15, 1));
            removedComponent1Subject.OnNext(new EntityChange(15, 1));
            
            Assert.NotEmpty(leavingInvocations);
            Assert.Equal(2, leavingInvocations.Count);
            Assert.Contains(50, leavingInvocations);
            Assert.Contains(15, leavingInvocations);      
            
            Assert.NotEmpty(leftInvocations);
            Assert.Equal(2, leftInvocations.Count);
            Assert.Contains(50, leftInvocations);
            Assert.Contains(15, leftInvocations);
        }

        [Fact]
        public void should_raise_joined_event_after_entity_leaves_and_rejoins_group()
        {
            var lookupGroup = new LookupGroup(new[] { 1 }, Array.Empty<int>());

            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            var addingComponent1Subject = new Subject<EntityChange>();
            var removingComponent1Subject = new Subject<EntityChange>();
            var removedComponent1Subject = new Subject<EntityChange>();
            entityChangeRouter.OnEntityAddedComponent(1).Returns(addingComponent1Subject);
            entityChangeRouter.OnEntityRemovingComponent(1).Returns(removingComponent1Subject);
            entityChangeRouter.OnEntityRemovedComponent(1).Returns(removedComponent1Subject);
            var groupTracker = new EntityRouterObservableGroupTracker(entityChangeRouter, lookupGroup);
            
            var joinedInvocations = new List<int>();
            groupTracker.OnEntityJoinedGroup.Subscribe(joinedInvocations.Add);
            
            var leavingInvocations = new List<int>();
            groupTracker.OnEntityLeavingGroup.Subscribe(leavingInvocations.Add);
            
            var leftInvocations = new List<int>();
            groupTracker.OnEntityLeftGroup.Subscribe(leftInvocations.Add);
            
            // 50 and 15 are presumed added, 12, 16 not yet
            groupTracker.EntityIdMatchState.Add(50, new GroupMatchingState(0, 0));
            
            removingComponent1Subject.OnNext(new EntityChange(50, 1));
            removedComponent1Subject.OnNext(new EntityChange(50, 1));
            addingComponent1Subject.OnNext(new EntityChange(50, 1));
            removingComponent1Subject.OnNext(new EntityChange(50, 1));
            removedComponent1Subject.OnNext(new EntityChange(50, 1));
            addingComponent1Subject.OnNext(new EntityChange(50, 1));
            
            Assert.NotEmpty(joinedInvocations);
            Assert.Equal(2, joinedInvocations.Count);
            Assert.Contains(50, joinedInvocations);
            Assert.Contains(50, joinedInvocations);      
            
            Assert.NotEmpty(leavingInvocations);
            Assert.Equal(2, leavingInvocations.Count);
            Assert.Contains(50, leavingInvocations);
            Assert.Contains(50, leavingInvocations);      
            
            Assert.NotEmpty(leftInvocations);
            Assert.Equal(2, leftInvocations.Count);
            Assert.Contains(50, leftInvocations);
            Assert.Contains(50, leftInvocations);
        }
        
        [Fact]
        public void should_not_raise_joined_event_for_meeting_needed_components_until_no_longer_contains_excluded_component()
        {
            var lookupGroup = new LookupGroup(new[] { 1 }, new [] { 2 });

            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            var addingComponent1Subject = new Subject<EntityChange>();
            var addingComponent2Subject = new Subject<EntityChange>();
            var removingComponent2Subject = new Subject<EntityChange>();
            var removedComponent2Subject = new Subject<EntityChange>();
            entityChangeRouter.OnEntityAddedComponent(1).Returns(addingComponent1Subject);
            entityChangeRouter.OnEntityRemovingComponent(1).Returns(new Subject<EntityChange>());;
            entityChangeRouter.OnEntityRemovedComponent(1).Returns(new Subject<EntityChange>());
            entityChangeRouter.OnEntityRemovingComponent(2).Returns(removingComponent2Subject);
            entityChangeRouter.OnEntityRemovedComponent(2).Returns(removedComponent2Subject);
            entityChangeRouter.OnEntityAddedComponent(2).Returns(addingComponent2Subject);
            var groupTracker = new EntityRouterObservableGroupTracker(entityChangeRouter, lookupGroup);
            
            var joinedInvocations = new List<int>();
            groupTracker.OnEntityJoinedGroup.Subscribe(joinedInvocations.Add);
            
            var leavingInvocations = new List<int>();
            groupTracker.OnEntityLeavingGroup.Subscribe(leavingInvocations.Add);
            
            var leftInvocations = new List<int>();
            groupTracker.OnEntityLeftGroup.Subscribe(leftInvocations.Add);
            
            addingComponent2Subject.OnNext(new EntityChange(50, 2));
            addingComponent1Subject.OnNext(new EntityChange(50, 1));
            
            Assert.Empty(joinedInvocations);
            
            removingComponent2Subject.OnNext(new EntityChange(50, 2));
            removedComponent2Subject.OnNext(new EntityChange(50, 2));
            
            Assert.NotEmpty(joinedInvocations);
            Assert.Single(joinedInvocations, 50);      
            
            Assert.Empty(leavingInvocations);      
            Assert.Empty(leftInvocations);
        }
    }
}