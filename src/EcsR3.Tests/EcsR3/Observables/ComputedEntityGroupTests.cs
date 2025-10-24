using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections.Entities;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Groups.Tracking.Trackers;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsR3.Observables
{
    public class ComputedEntityGroupTests
    {
        [Fact]
        public void should_include_matching_entity_snapshot_on_creation()
        {
            var entity1 = new Entity(1, 0);
            var entity2 = new Entity(2, 0);
            var entity3 = new Entity(3, 0);
            var ownedEntities = new List<Entity> { entity1, entity2 };
            
            var accessorToken = new LookupGroup(new[]{1}, Array.Empty<int>());
   
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.GetMatchedEntities().Returns(ownedEntities);
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<Entity>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(new Subject<Entity>());
            mockGroupTracker.OnEntityLeftGroup.Returns(new Subject<Entity>());
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEnumerator().Returns(ownedEntities.GetEnumerator());
    
            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            
            Assert.Equal(2, observableGroup.CachedEntities.Count);
            Assert.Contains(entity1, observableGroup.CachedEntities);
            Assert.Contains(entity2, observableGroup.CachedEntities);
        }

        [Fact]
        public void should_add_entity_and_raise_event_when_applicable_entity_joined()
        {
            var group = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var entity1 = new Entity(1, 0);
            var entities = new List<Entity> { entity1 };

            var onJoinedSubject = new Subject<Entity>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(onJoinedSubject);
            mockGroupTracker.OnEntityLeavingGroup.Returns(new Subject<Entity>());
            mockGroupTracker.OnEntityLeftGroup.Returns(new Subject<Entity>());
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEnumerator().Returns(entities.GetEnumerator());

            var observableGroup = new ComputedEntityGroup(group, mockGroupTracker, mockEntityCollection);

            var invocations = new List<Entity>();
            observableGroup.OnAdded.Subscribe(invocations.Add);
            
            onJoinedSubject.OnNext(entity1);

            Assert.NotEmpty(invocations);
            Assert.Single(observableGroup.CachedEntities, entity1);
            Assert.Single(invocations, entity1);
        }
        
        [Fact]
        public void should_remove_entity_and_raise_event_when_applicable_entity_removed()
        {
            var accessorToken = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var entity1 = new Entity(1, 0);
            var entities = new List<Entity> { entity1 };

            var onLeavingSubject = new Subject<Entity>();
            var onLeftSubject = new Subject<Entity>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<Entity>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(onLeavingSubject);
            mockGroupTracker.OnEntityLeftGroup.Returns(onLeftSubject);
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEnumerator().Returns(entities.GetEnumerator());

            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            observableGroup.CachedEntities.Add(entity1);
            
            var removingInvocations = new List<Entity>();
            observableGroup.OnRemoving.Subscribe(removingInvocations.Add);
            
            var removedInvocations = new List<Entity>();
            observableGroup.OnRemoved.Subscribe(removedInvocations.Add);
            
            onLeavingSubject.OnNext(entity1);
            onLeftSubject.OnNext(entity1);

            Assert.NotEmpty(removingInvocations);
            Assert.Single(removingInvocations, entity1);
            Assert.NotEmpty(removedInvocations);
            Assert.Single(removedInvocations, entity1);
            Assert.Empty(observableGroup.CachedEntities);
        }
        
        [Fact]
        public void should_not_remove_entity_and_raise_event_when_applicable_entity_removing()
        {
            var accessorToken = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var entity1 = new Entity(1, 0);
            var entities = new List<Entity> { entity1 };
            
            var onLeavingSubject = new Subject<Entity>();
            var onLeftSubject = new Subject<Entity>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<Entity>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(onLeavingSubject);
            mockGroupTracker.OnEntityLeftGroup.Returns(onLeftSubject);
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEnumerator().Returns(entities.GetEnumerator());

            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            observableGroup.CachedEntities.Add(entity1);
            
            var removingInvocations = new List<Entity>();
            observableGroup.OnRemoving.Subscribe(removingInvocations.Add);
            
            var removedInvocations = new List<Entity>();
            observableGroup.OnRemoved.Subscribe(removedInvocations.Add);
            
            onLeavingSubject.OnNext(entity1);

            Assert.NotEmpty(removingInvocations);
            Assert.Single(removingInvocations, entity1);
            Assert.Empty(removedInvocations);
            Assert.Single(observableGroup.CachedEntities, entity1);
        }
        
        [Fact]
        public void should_notify_change_on_adding_or_removed_happening()
        {
            var accessorToken = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var entity1 = new Entity(1, 0);
            var entities = new List<Entity> { entity1 };

            var onJoinedSubject = new Subject<Entity>();
            var onLeftSubject = new Subject<Entity>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(onJoinedSubject);
            mockGroupTracker.OnEntityLeavingGroup.Returns(new Subject<Entity>());
            mockGroupTracker.OnEntityLeftGroup.Returns(onLeftSubject);
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);

            var changeInvocations = new List<Entity[]>();
            observableGroup.OnChanged.Subscribe(x => changeInvocations.Add(x.ToArray()));
            
            onJoinedSubject.OnNext(entity1);
            onLeftSubject.OnNext(entity1);

            Assert.NotEmpty(changeInvocations);
            Assert.Equal(2, changeInvocations.Count);
            Assert.Equal(1, changeInvocations[0].Length);
            Assert.Equal(0, changeInvocations[1].Length);
            Assert.Empty(observableGroup.CachedEntities);
        }
    }
}
