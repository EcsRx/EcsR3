using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Entities.Routing;
using EcsR3.Groups;
using EcsR3.Groups.Tracking.Trackers;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsR3.Observables.Trackers
{
    public class EntityRouterObservableGroupTrackerTests
    {
        [Fact]
        public void should_start_listening_for_required_changes_when_created()
        {
            // 1,2 are required, 3 is excluded
            var lookupGroup = new LookupGroup(new[] { 1,2 }, new [] {3});

            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            entityChangeRouter.OnEntityAddedComponents(Arg.Any<int[]>()).Returns(new Subject<EntityChanges>());
            entityChangeRouter.OnEntityRemovingComponents(Arg.Any<int[]>()).Returns(new Subject<EntityChanges>());
            entityChangeRouter.OnEntityRemovedComponents(Arg.Any<int[]>()).Returns(new Subject<EntityChanges>());
            var groupTracker = new EntityRouterObservableGroupTracker(entityChangeRouter, lookupGroup);

            entityChangeRouter.Received(1).OnEntityAddedComponents(Arg.Is<int[]>(x => x.Length == 2 && x.Contains(1) && x.Contains(2)));
            entityChangeRouter.Received(1).OnEntityAddedComponents(Arg.Is<int[]>(x => x.Length == 1 && x.Contains(3)));
            entityChangeRouter.Received(1).OnEntityRemovingComponents(Arg.Is<int[]>(x => x.Length == 2 && x.Contains(1) && x.Contains(2)));
            entityChangeRouter.Received(1).OnEntityRemovedComponents(Arg.Is<int[]>(x => x.Length == 2 && x.Length == 2 && x.Contains(1) && x.Contains(2)));
            entityChangeRouter.Received(1).OnEntityRemovedComponents(Arg.Is<int[]>(x => x.Length == 1 && x.Contains(3)));
        }
        
        [Fact]
        public void should_raise_joined_event_when_entity_adds_needed_components()
        {
            // 1, 2 required
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            var addedRequiredComponentsSubject = new Subject<EntityChanges>();
            var removingRequiredComponentsSubject = new Subject<EntityChanges>();
            var removedRequiredComponentsSubject = new Subject<EntityChanges>();
            entityChangeRouter
                .OnEntityAddedComponents(Arg.Is<int[]>(x => x.Length == 2 && x.Contains(1) && x.Contains(2)))
                .Returns(addedRequiredComponentsSubject);
            
            entityChangeRouter.OnEntityRemovingComponents(Arg.Any<int[]>()).Returns(removingRequiredComponentsSubject);
            entityChangeRouter.OnEntityRemovedComponents(Arg.Any<int[]>()).Returns(removedRequiredComponentsSubject);
            var groupTracker = new EntityRouterObservableGroupTracker(entityChangeRouter, lookupGroup);

            var joinedInvocations = new List<int>();
            groupTracker.OnEntityJoinedGroup.Subscribe(joinedInvocations.Add);
            var leavingInvocations = new List<int>();
            groupTracker.OnEntityLeavingGroup.Subscribe(leavingInvocations.Add);
            var leftInvocations = new List<int>();
            groupTracker.OnEntityLeftGroup.Subscribe(leftInvocations.Add);
            
            addedRequiredComponentsSubject.OnNext(new EntityChanges(50, new[]{1,2}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(12, new[]{1}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(16, new[]{2}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(15, new[]{2}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(15, new[]{1}));
            
            Assert.NotEmpty(joinedInvocations);
            Assert.Equal(2, joinedInvocations.Count);
            Assert.Contains(50, joinedInvocations);
            Assert.Contains(15, joinedInvocations);
            
            Assert.Empty(leavingInvocations);
            Assert.Empty(leftInvocations);
        }
        
        [Fact]
        public void should_raise_leaving_and_left_event_when_entity_removes_needed_components()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            var addedRequiredComponentsSubject = new Subject<EntityChanges>();
            var removingRequiredComponentsSubject = new Subject<EntityChanges>();
            var removedRequiredComponentsSubject = new Subject<EntityChanges>();
            entityChangeRouter
                .OnEntityAddedComponents(Arg.Any<int[]>()).Returns(addedRequiredComponentsSubject);
            
            entityChangeRouter
                .OnEntityRemovingComponents(Arg.Is<int[]>(x => x.Length == 2 && x.Contains(1) && x.Contains(2)))
                .Returns(removingRequiredComponentsSubject);
            
            entityChangeRouter
                .OnEntityRemovedComponents(Arg.Is<int[]>(x => x.Length == 2 && x.Contains(1) && x.Contains(2)))
                .Returns(removedRequiredComponentsSubject);

            var groupTracker = new EntityRouterObservableGroupTracker(entityChangeRouter, lookupGroup);

            var joinedInvocations = new List<int>();
            groupTracker.OnEntityJoinedGroup.Subscribe(joinedInvocations.Add);
            
            var leavingInvocations = new List<int>();
            groupTracker.OnEntityLeavingGroup.Subscribe(leavingInvocations.Add);
            
            var leftInvocations = new List<int>();
            groupTracker.OnEntityLeftGroup.Subscribe(leftInvocations.Add);
            
            // 50 and 15 are presumed added, 12, 16 not yet
            groupTracker.EntityIdMatchState.Add(50, new GroupMatchingState(0, 0));
            groupTracker.EntityIdMatchState.Add(12, new GroupMatchingState(1, 0));
            groupTracker.EntityIdMatchState.Add(16, new GroupMatchingState(1, 0));
            groupTracker.EntityIdMatchState.Add(15, new GroupMatchingState(0, 0));
            
            removingRequiredComponentsSubject.OnNext(new EntityChanges(50, new[]{1,2}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(50, new[]{1,2}));
            removingRequiredComponentsSubject.OnNext(new EntityChanges(12, new[]{1}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(12, new[]{1}));
            removingRequiredComponentsSubject.OnNext(new EntityChanges(16, new[]{2}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(16, new[]{2}));
            removingRequiredComponentsSubject.OnNext(new EntityChanges(15, new[]{2}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(15, new[]{2}));
            removingRequiredComponentsSubject.OnNext(new EntityChanges(15, new[]{1}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(15, new[]{1}));
            
            Assert.Empty(joinedInvocations);
            
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
            var addedRequiredComponentsSubject = new Subject<EntityChanges>();
            var removingRequiredComponentsSubject = new Subject<EntityChanges>();
            var removedRequiredComponentsSubject = new Subject<EntityChanges>();
            entityChangeRouter.OnEntityAddedComponents(Arg.Any<int[]>()).Returns(addedRequiredComponentsSubject);
            entityChangeRouter.OnEntityRemovingComponents(Arg.Any<int[]>()).Returns(removingRequiredComponentsSubject);
            entityChangeRouter.OnEntityRemovedComponents(Arg.Any<int[]>()).Returns(removedRequiredComponentsSubject);
            var groupTracker = new EntityRouterObservableGroupTracker(entityChangeRouter, lookupGroup);
            
            var joinedInvocations = new List<int>();
            groupTracker.OnEntityJoinedGroup.Subscribe(joinedInvocations.Add);
            
            var leavingInvocations = new List<int>();
            groupTracker.OnEntityLeavingGroup.Subscribe(leavingInvocations.Add);
            
            var leftInvocations = new List<int>();
            groupTracker.OnEntityLeftGroup.Subscribe(leftInvocations.Add);
            
            // 50 and 15 are presumed added, 12, 16 not yet
            groupTracker.EntityIdMatchState.Add(50, new GroupMatchingState(0, 0));
            
            removingRequiredComponentsSubject.OnNext(new EntityChanges(50, new[]{1}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(50, new[]{1}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(50, new[]{1}));
            removingRequiredComponentsSubject.OnNext(new EntityChanges(50, new[]{1}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(50, new[]{1}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(50, new[]{1}));
            
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
        public void should_not_raise_joined_event_for_meeting_needed_components_until_no_longer_contains_excluded_components()
        {
            var lookupGroup = new LookupGroup(new[] { 1 }, new [] { 2 });

            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            var addedRequiredComponentsSubject = new Subject<EntityChanges>();
            var addedExcludedComponentsSubject = new Subject<EntityChanges>();
            var removingRequiredComponentsSubject = new Subject<EntityChanges>();
            var removedRequiredComponentsSubject = new Subject<EntityChanges>();
            var removedExcludedComponentsSubject = new Subject<EntityChanges>();

            entityChangeRouter
                .OnEntityAddedComponents(Arg.Is<int[]>(x => x.Length == 1 && x.Contains(1)))
                .Returns(addedRequiredComponentsSubject);
            
            entityChangeRouter
                .OnEntityAddedComponents(Arg.Is<int[]>(x => x.Length == 1 && x.Contains(2)))
                .Returns(addedExcludedComponentsSubject);
            
            entityChangeRouter
                .OnEntityRemovingComponents(Arg.Is<int[]>(x => x.Length == 1 && x.Contains(1)))
                .Returns(removingRequiredComponentsSubject);
            
            entityChangeRouter
                .OnEntityRemovedComponents(Arg.Is<int[]>(x => x.Length == 1 && x.Contains(1)))
                .Returns(removedRequiredComponentsSubject);
            
            entityChangeRouter
                .OnEntityRemovedComponents(Arg.Is<int[]>(x => x.Length == 1 && x.Contains(2)))
                .Returns(removedExcludedComponentsSubject);
            
            var groupTracker = new EntityRouterObservableGroupTracker(entityChangeRouter, lookupGroup);
            
            var joinedInvocations = new List<int>();
            groupTracker.OnEntityJoinedGroup.Subscribe(joinedInvocations.Add);
            
            var leavingInvocations = new List<int>();
            groupTracker.OnEntityLeavingGroup.Subscribe(leavingInvocations.Add);
            
            var leftInvocations = new List<int>();
            groupTracker.OnEntityLeftGroup.Subscribe(leftInvocations.Add);
            
            addedExcludedComponentsSubject.OnNext(new EntityChanges(50, new[]{2}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(50, new[]{1}));
            
            Assert.Empty(joinedInvocations);
            Assert.Empty(leavingInvocations);
            Assert.Empty(leftInvocations);
            
            removedExcludedComponentsSubject.OnNext(new EntityChanges(50, new[]{2}));
            
            Assert.NotEmpty(joinedInvocations);
            Assert.Single(joinedInvocations, 50);      
            
            Assert.Empty(leavingInvocations);      
            Assert.Empty(leftInvocations);
            
            addedExcludedComponentsSubject.OnNext(new EntityChanges(50, new[]{2}));
            
            Assert.NotEmpty(leavingInvocations);
            Assert.Single(leavingInvocations, 50);    
            
            Assert.NotEmpty(leftInvocations);
            Assert.Single(leftInvocations, 50);   
        }
    }
}