using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Entities;
using EcsR3.Entities.Routing;
using EcsR3.Groups;
using EcsR3.Groups.Tracking.Trackers;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsR3.Observables.Trackers
{
    public class ComputedEntityGroupTrackerTests
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
            var groupTracker = new ComputedEntityGroupTracker(entityChangeRouter, lookupGroup);

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
            var groupTracker = new ComputedEntityGroupTracker(entityChangeRouter, lookupGroup);

            var joinedInvocations = new List<Entity>();
            groupTracker.OnEntityJoinedGroup.Subscribe(joinedInvocations.Add);
            var leavingInvocations = new List<Entity>();
            groupTracker.OnEntityLeavingGroup.Subscribe(leavingInvocations.Add);
            var leftInvocations = new List<Entity>();
            groupTracker.OnEntityLeftGroup.Subscribe(leftInvocations.Add);

            var entity = new Entity(50, 0);
            var entity2 = new Entity(15, 0);
            
            addedRequiredComponentsSubject.OnNext(new EntityChanges(entity, new[]{1,2}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(new Entity(12, 0), new[]{1}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(new Entity(16, 0), new[]{2}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(entity2, new[]{2}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(entity2, new[]{1}));
            
            Assert.NotEmpty(joinedInvocations);
            Assert.Equal(2, joinedInvocations.Count);
            Assert.Contains(entity, joinedInvocations);
            Assert.Contains(entity2, joinedInvocations);
            
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

            var groupTracker = new ComputedEntityGroupTracker(entityChangeRouter, lookupGroup);

            var joinedInvocations = new List<Entity>();
            groupTracker.OnEntityJoinedGroup.Subscribe(joinedInvocations.Add);
            
            var leavingInvocations = new List<Entity>();
            groupTracker.OnEntityLeavingGroup.Subscribe(leavingInvocations.Add);
            
            var leftInvocations = new List<Entity>();
            groupTracker.OnEntityLeftGroup.Subscribe(leftInvocations.Add);
            
            var entity1 = new Entity(50, 0);
            var entity2 = new Entity(12, 0);
            var entity3 = new Entity(16, 0);
            var entity4 = new Entity(15, 0);
            
            // 50 and 15 are presumed added, 12, 16 not yet
            groupTracker.EntityIdMatchState.Add(entity1, new GroupMatchingState(0, 0));
            groupTracker.EntityIdMatchState.Add(entity2, new GroupMatchingState(1, 0));
            groupTracker.EntityIdMatchState.Add(entity3, new GroupMatchingState(1, 0));
            groupTracker.EntityIdMatchState.Add(entity4, new GroupMatchingState(0, 0));
            
            removingRequiredComponentsSubject.OnNext(new EntityChanges(entity1, new[]{1,2}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(entity1, new[]{1,2}));
            removingRequiredComponentsSubject.OnNext(new EntityChanges(entity2, new[]{1}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(entity2, new[]{1}));
            removingRequiredComponentsSubject.OnNext(new EntityChanges(entity3, new[]{2}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(entity3, new[]{2}));
            removingRequiredComponentsSubject.OnNext(new EntityChanges(entity4, new[]{2}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(entity4, new[]{2}));
            removingRequiredComponentsSubject.OnNext(new EntityChanges(entity4, new[]{1}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(entity4, new[]{1}));
            
            Assert.Empty(joinedInvocations);
            
            Assert.NotEmpty(leavingInvocations);
            Assert.Equal(2, leavingInvocations.Count);
            Assert.Contains(entity1, leavingInvocations);
            Assert.Contains(entity4, leavingInvocations);      
            
            Assert.NotEmpty(leftInvocations);
            Assert.Equal(2, leftInvocations.Count);
            Assert.Contains(entity1, leftInvocations);
            Assert.Contains(entity4, leftInvocations);
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
            var groupTracker = new ComputedEntityGroupTracker(entityChangeRouter, lookupGroup);
            
            var joinedInvocations = new List<Entity>();
            groupTracker.OnEntityJoinedGroup.Subscribe(joinedInvocations.Add);
            
            var leavingInvocations = new List<Entity>();
            groupTracker.OnEntityLeavingGroup.Subscribe(leavingInvocations.Add);
            
            var leftInvocations = new List<Entity>();
            groupTracker.OnEntityLeftGroup.Subscribe(leftInvocations.Add);
            
            var entity = new Entity(50, 0);
            // 50 and 15 are presumed added, 12, 16 not yet
            groupTracker.EntityIdMatchState.Add(entity, new GroupMatchingState(0, 0));
            
            removingRequiredComponentsSubject.OnNext(new EntityChanges(entity, new[]{1}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(entity, new[]{1}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(entity, new[]{1}));
            removingRequiredComponentsSubject.OnNext(new EntityChanges(entity, new[]{1}));
            removedRequiredComponentsSubject.OnNext(new EntityChanges(entity, new[]{1}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(entity, new[]{1}));
            
            Assert.NotEmpty(joinedInvocations);
            Assert.Equal(2, joinedInvocations.Count);
            Assert.Contains(entity, joinedInvocations);
            Assert.Contains(entity, joinedInvocations);      
            
            Assert.NotEmpty(leavingInvocations);
            Assert.Equal(2, leavingInvocations.Count);
            Assert.Contains(entity, leavingInvocations);
            Assert.Contains(entity, leavingInvocations);      
            
            Assert.NotEmpty(leftInvocations);
            Assert.Equal(2, leftInvocations.Count);
            Assert.Contains(entity, leftInvocations);
            Assert.Contains(entity, leftInvocations);
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
            
            var groupTracker = new ComputedEntityGroupTracker(entityChangeRouter, lookupGroup);
            
            var joinedInvocations = new List<Entity>();
            groupTracker.OnEntityJoinedGroup.Subscribe(joinedInvocations.Add);
            
            var leavingInvocations = new List<Entity>();
            groupTracker.OnEntityLeavingGroup.Subscribe(leavingInvocations.Add);
            
            var leftInvocations = new List<Entity>();
            groupTracker.OnEntityLeftGroup.Subscribe(leftInvocations.Add);

            var entity = new Entity(50, 0);
            addedExcludedComponentsSubject.OnNext(new EntityChanges(entity, new[]{2}));
            addedRequiredComponentsSubject.OnNext(new EntityChanges(entity, new[]{1}));
            
            Assert.Empty(joinedInvocations);
            Assert.Empty(leavingInvocations);
            Assert.Empty(leftInvocations);
            
            removedExcludedComponentsSubject.OnNext(new EntityChanges(entity, new[]{2}));
            
            Assert.NotEmpty(joinedInvocations);
            Assert.Single(joinedInvocations, entity);      
            
            Assert.Empty(leavingInvocations);      
            Assert.Empty(leftInvocations);
            
            addedExcludedComponentsSubject.OnNext(new EntityChanges(entity, new[]{2}));
            
            Assert.NotEmpty(leavingInvocations);
            Assert.Single(leavingInvocations, entity);    
            
            Assert.NotEmpty(leftInvocations);
            Assert.Single(leftInvocations, entity);   
        }
    }
}