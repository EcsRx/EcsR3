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
            var id1 = 1;
            var id2 = 2;
            var id3 = 3;
            
            var accessorToken = new LookupGroup(new[]{1}, Array.Empty<int>());
            var ownedEntities = new List<int> { id1, id2 };
   
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.GetMatchedEntityIds().Returns(ownedEntities);
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeftGroup.Returns(new Subject<int>());
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEnumerator().Returns(ownedEntities.GetEnumerator());
    
            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            
            Assert.Equal(2, observableGroup.CachedEntityIds.Count);
            Assert.Contains(id1, observableGroup.CachedEntityIds);
            Assert.Contains(id2, observableGroup.CachedEntityIds);
        }

        [Fact]
        public void should_add_entity_and_raise_event_when_applicable_entity_joined()
        {
            var group = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var id1 = 1;
            var entities = new List<int> { id1 };

            var onJoinedSubject = new Subject<int>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(onJoinedSubject);
            mockGroupTracker.OnEntityLeavingGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeftGroup.Returns(new Subject<int>());
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEnumerator().Returns(entities.GetEnumerator());

            var observableGroup = new ComputedEntityGroup(group, mockGroupTracker, mockEntityCollection);

            var invocations = new List<int>();
            observableGroup.OnAdded.Subscribe(invocations.Add);
            
            onJoinedSubject.OnNext(id1);

            Assert.NotEmpty(invocations);
            Assert.Single(observableGroup.CachedEntityIds, id1);
            Assert.Single(invocations, id1);
        }
        
        [Fact]
        public void should_remove_entity_and_raise_event_when_applicable_entity_removed()
        {
            var accessorToken = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var id1 = 1;
            var entities = new List<int> { id1 };

            var onLeavingSubject = new Subject<int>();
            var onLeftSubject = new Subject<int>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(onLeavingSubject);
            mockGroupTracker.OnEntityLeftGroup.Returns(onLeftSubject);
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEnumerator().Returns(entities.GetEnumerator());

            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            observableGroup.CachedEntityIds.Add(id1);
            
            var removingInvocations = new List<int>();
            observableGroup.OnRemoving.Subscribe(removingInvocations.Add);
            
            var removedInvocations = new List<int>();
            observableGroup.OnRemoved.Subscribe(removedInvocations.Add);
            
            onLeavingSubject.OnNext(id1);
            onLeftSubject.OnNext(id1);

            Assert.NotEmpty(removingInvocations);
            Assert.Single(removingInvocations, id1);
            Assert.NotEmpty(removedInvocations);
            Assert.Single(removedInvocations, id1);
            Assert.Empty(observableGroup.CachedEntityIds);
        }
        
        [Fact]
        public void should_not_remove_entity_and_raise_event_when_applicable_entity_removing()
        {
            var accessorToken = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var id1 = 1;
            var entities = new List<int> { id1 };
            
            var onLeavingSubject = new Subject<int>();
            var onLeftSubject = new Subject<int>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(onLeavingSubject);
            mockGroupTracker.OnEntityLeftGroup.Returns(onLeftSubject);
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEnumerator().Returns(entities.GetEnumerator());

            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            observableGroup.CachedEntityIds.Add(1);
            
            var removingInvocations = new List<int>();
            observableGroup.OnRemoving.Subscribe(removingInvocations.Add);
            
            var removedInvocations = new List<int>();
            observableGroup.OnRemoved.Subscribe(removedInvocations.Add);
            
            onLeavingSubject.OnNext(id1);

            Assert.NotEmpty(removingInvocations);
            Assert.Single(removingInvocations, id1);
            Assert.Empty(removedInvocations);
            Assert.Single(observableGroup.CachedEntityIds, id1);
        }
        
        [Fact]
        public void should_notify_change_on_adding_or_removed_happening()
        {
            var accessorToken = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var id1 = 1;
            var entities = new List<int> { id1 };

            var onJoinedSubject = new Subject<int>();
            var onLeftSubject = new Subject<int>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(onJoinedSubject);
            mockGroupTracker.OnEntityLeavingGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeftGroup.Returns(onLeftSubject);
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);

            var changeInvocations = new List<int[]>();
            observableGroup.OnChanged.Subscribe(x => changeInvocations.Add(x.ToArray()));
            
            onJoinedSubject.OnNext(id1);
            onLeftSubject.OnNext(id1);

            Assert.NotEmpty(changeInvocations);
            Assert.Equal(2, changeInvocations.Count);
            Assert.Equal(1, changeInvocations[0].Length);
            Assert.Equal(0, changeInvocations[1].Length);
            Assert.Empty(observableGroup.CachedEntityIds);
        }
    }
}
