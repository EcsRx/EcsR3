using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections.Entity;
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
            var accessorToken = new LookupGroup(new[]{1}, Array.Empty<int>());

            var applicableEntity1 = Substitute.For<IEntity>();
            var applicableEntity2 = Substitute.For<IEntity>();
            var notApplicableEntity1 = Substitute.For<IEntity>();

            applicableEntity1.Id.Returns(1);
            applicableEntity2.Id.Returns(2);
            notApplicableEntity1.Id.Returns(3);
   
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.GetMatchedEntityIds().Returns(new [] { 1, 2 });
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeftGroup.Returns(new Subject<int>());
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.Get(1).Returns(applicableEntity1);
            mockEntityCollection.Get(2).Returns(applicableEntity2);
    
            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            
            Assert.Equal(2, observableGroup.CachedEntityIds.Count);
            Assert.Contains(applicableEntity1.Id, observableGroup.CachedEntityIds);
            Assert.Contains(applicableEntity2.Id, observableGroup.CachedEntityIds);
        }

        [Fact]
        public void should_add_entity_and_raise_event_when_applicable_entity_joined()
        {
            var group = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);

            var onJoinedSubject = new Subject<int>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(onJoinedSubject);
            mockGroupTracker.OnEntityLeavingGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeftGroup.Returns(new Subject<int>());
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.Get(1).Returns(applicableEntity);

            var observableGroup = new ComputedEntityGroup(group, mockGroupTracker, mockEntityCollection);

            var invocations = new List<IEntity>();
            observableGroup.OnAdded.Subscribe(invocations.Add);
            
            onJoinedSubject.OnNext(applicableEntity.Id);

            Assert.NotEmpty(invocations);
            Assert.Single(observableGroup.CachedEntityIds, applicableEntity.Id);
            Assert.Single(invocations, applicableEntity);
        }
        
        [Fact]
        public void should_remove_entity_and_raise_event_when_applicable_entity_removed()
        {
            var accessorToken = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);

            var onLeavingSubject = new Subject<int>();
            var onLeftSubject = new Subject<int>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(onLeavingSubject);
            mockGroupTracker.OnEntityLeftGroup.Returns(onLeftSubject);
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.Get(1).Returns(applicableEntity);

            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            observableGroup.CachedEntityIds.Add(1);
            
            var removingInvocations = new List<IEntity>();
            observableGroup.OnRemoving.Subscribe(removingInvocations.Add);
            
            var removedInvocations = new List<IEntity>();
            observableGroup.OnRemoved.Subscribe(removedInvocations.Add);
            
            onLeavingSubject.OnNext(applicableEntity.Id);
            onLeftSubject.OnNext(applicableEntity.Id);

            Assert.NotEmpty(removingInvocations);
            Assert.Single(removingInvocations, applicableEntity);
            Assert.NotEmpty(removedInvocations);
            Assert.Single(removedInvocations, applicableEntity);
            Assert.Empty(observableGroup.CachedEntityIds);
        }
        
        [Fact]
        public void should_not_remove_entity_and_raise_event_when_applicable_entity_removing()
        {
            var accessorToken = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);

            var onLeavingSubject = new Subject<int>();
            var onLeftSubject = new Subject<int>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(onLeavingSubject);
            mockGroupTracker.OnEntityLeftGroup.Returns(onLeftSubject);
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.Get(1).Returns(applicableEntity);

            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            observableGroup.CachedEntityIds.Add(1);
            
            var removingInvocations = new List<IEntity>();
            observableGroup.OnRemoving.Subscribe(removingInvocations.Add);
            
            var removedInvocations = new List<IEntity>();
            observableGroup.OnRemoved.Subscribe(removedInvocations.Add);
            
            onLeavingSubject.OnNext(applicableEntity.Id);

            Assert.NotEmpty(removingInvocations);
            Assert.Single(removingInvocations, applicableEntity);
            Assert.Empty(removedInvocations);
            Assert.Single(observableGroup.CachedEntityIds, applicableEntity.Id);
        }
        
        [Fact]
        public void should_notify_change_on_adding_or_removed_happening()
        {
            var accessorToken = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);

            var onJoinedSubject = new Subject<int>();
            var onLeftSubject = new Subject<int>();
            var mockGroupTracker = Substitute.For<IComputedEntityGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(onJoinedSubject);
            mockGroupTracker.OnEntityLeavingGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeftGroup.Returns(onLeftSubject);
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            var observableGroup = new ComputedEntityGroup(accessorToken, mockGroupTracker, mockEntityCollection);

            var changeInvocations = new List<IEntity[]>();
            observableGroup.OnChanged.Subscribe(x => changeInvocations.Add(x.ToArray()));
            
            onJoinedSubject.OnNext(applicableEntity.Id);
            onLeftSubject.OnNext(applicableEntity.Id);

            Assert.NotEmpty(changeInvocations);
            Assert.Equal(2, changeInvocations.Count);
            Assert.Equal(1, changeInvocations[0].Length);
            Assert.Equal(0, changeInvocations[1].Length);
            Assert.Empty(observableGroup.CachedEntityIds);
        }
    }
}
