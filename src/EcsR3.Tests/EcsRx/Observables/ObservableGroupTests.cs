using System;
using System.Collections.Generic;
using EcsR3.Collections.Entity;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using EcsR3.Groups.Observable.Tracking.Events;
using EcsR3.Groups.Observable.Tracking.Trackers;
using EcsR3.Groups.Observable.Tracking.Types;
using NSubstitute;
using R3;
using Xunit;


namespace EcsR3.Tests.EcsRx.Observables
{
    public class ObservableGroupTests
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
   
            var mockGroupTracker = Substitute.For<IObservableGroupTracker>();
            mockGroupTracker.GetMatchedEntityIds().Returns(new [] { 1, 2 });
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeftGroup.Returns(new Subject<int>());
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEntity(1).Returns(applicableEntity1);
            mockEntityCollection.GetEntity(2).Returns(applicableEntity2);
    
            var observableGroup = new ObservableGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            
            Assert.Equal(2, observableGroup.CachedEntities.Count);
            Assert.Contains(applicableEntity1, observableGroup.CachedEntities);
            Assert.Contains(applicableEntity2, observableGroup.CachedEntities);
        }

        [Fact]
        public void should_add_entity_and_raise_event_when_applicable_entity_joined()
        {
            var accessorToken = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);

            var onJoinedSubject = new Subject<int>();
            var mockGroupTracker = Substitute.For<IObservableGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(onJoinedSubject);
            mockGroupTracker.OnEntityLeavingGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeftGroup.Returns(new Subject<int>());
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEntity(1).Returns(applicableEntity);

            var observableGroup = new ObservableGroup(accessorToken, mockGroupTracker, mockEntityCollection);

            var invocations = new List<IEntity>();
            observableGroup.OnEntityAdded.Subscribe(invocations.Add);
            
            onJoinedSubject.OnNext(applicableEntity.Id);

            Assert.NotEmpty(invocations);
            Assert.Single(observableGroup.CachedEntities, applicableEntity);
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
            var mockGroupTracker = Substitute.For<IObservableGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(onLeavingSubject);
            mockGroupTracker.OnEntityLeftGroup.Returns(onLeftSubject);
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEntity(1).Returns(applicableEntity);

            var observableGroup = new ObservableGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            observableGroup.CachedEntities.Add(applicableEntity);
            
            var removingInvocations = new List<IEntity>();
            observableGroup.OnEntityRemoving.Subscribe(removingInvocations.Add);
            
            var removedInvocations = new List<IEntity>();
            observableGroup.OnEntityRemoved.Subscribe(removedInvocations.Add);
            
            onLeavingSubject.OnNext(applicableEntity.Id);
            onLeftSubject.OnNext(applicableEntity.Id);

            Assert.NotEmpty(removingInvocations);
            Assert.Single(removingInvocations, applicableEntity);
            Assert.NotEmpty(removedInvocations);
            Assert.Single(removedInvocations, applicableEntity);
            Assert.Empty(observableGroup.CachedEntities);
        }
        
        [Fact]
        public void should_not_remove_entity_and_raise_event_when_applicable_entity_removing()
        {
            var accessorToken = new LookupGroup(new[]{1, 2}, Array.Empty<int>());

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);

            var onLeavingSubject = new Subject<int>();
            var onLeftSubject = new Subject<int>();
            var mockGroupTracker = Substitute.For<IObservableGroupTracker>();
            mockGroupTracker.OnEntityJoinedGroup.Returns(new Subject<int>());
            mockGroupTracker.OnEntityLeavingGroup.Returns(onLeavingSubject);
            mockGroupTracker.OnEntityLeftGroup.Returns(onLeftSubject);
            
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.GetEntity(1).Returns(applicableEntity);

            var observableGroup = new ObservableGroup(accessorToken, mockGroupTracker, mockEntityCollection);
            observableGroup.CachedEntities.Add(applicableEntity);
            
            var removingInvocations = new List<IEntity>();
            observableGroup.OnEntityRemoving.Subscribe(removingInvocations.Add);
            
            var removedInvocations = new List<IEntity>();
            observableGroup.OnEntityRemoved.Subscribe(removedInvocations.Add);
            
            onLeavingSubject.OnNext(applicableEntity.Id);

            Assert.NotEmpty(removingInvocations);
            Assert.Single(removingInvocations, applicableEntity);
            Assert.Empty(removedInvocations);
            Assert.Single(observableGroup.CachedEntities, applicableEntity);
        }
    }
}
