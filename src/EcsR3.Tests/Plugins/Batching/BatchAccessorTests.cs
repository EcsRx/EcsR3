using System.Linq;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Plugins.Batching.Accessors;
using EcsR3.Plugins.Batching.Builders;
using EcsR3.Tests.Models;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.Plugins.Batching
{
    public class BatchAccessorTests
    {
        [Fact]
        public void should_populate_on_setup()
        {                      
            var mockTypeLookup = Substitute.For<IComponentTypeLookup>();
            mockTypeLookup.GetComponentTypeId(typeof(TestStructComponentOne)).Returns(0);
            mockTypeLookup.GetComponentTypeId(typeof(TestStructComponentTwo)).Returns(1);

            var mockComponentPool1 = Substitute.For<IComponentPool<TestStructComponentOne>>();
            mockComponentPool1.OnPoolExtending.Returns(Observable.Empty<bool>());
                
            var mockComponentPool2 = Substitute.For<IComponentPool<TestStructComponentTwo>>();
            mockComponentPool2.OnPoolExtending.Returns(Observable.Empty<bool>());

            var mockComponentDatabase = Substitute.For<IComponentDatabase>();
            mockComponentDatabase.GetPoolFor<TestStructComponentOne>(0).Returns(mockComponentPool1);
            mockComponentDatabase.GetPoolFor<TestStructComponentTwo>(1).Returns(mockComponentPool2);
            
            var mockObservableGroup = Substitute.For<IComputedEntityGroup>();
            mockObservableGroup.OnAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnRemoving.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnRemoved.Returns(Observable.Empty<IEntity>());
            
            var mockBatchBuilder = Substitute.For<IBatchBuilder<TestStructComponentOne, TestStructComponentTwo>>();
            
            var batchAccessor = new BatchAccessor<TestStructComponentOne,TestStructComponentTwo>(mockObservableGroup, mockComponentDatabase, mockBatchBuilder, mockTypeLookup);          
            mockBatchBuilder.Received(1).Build(mockObservableGroup.ToArray());
        }
        
        [Fact]
        public void should_update_when_observable_group_changes()
        {
            var mockTypeLookup = Substitute.For<IComponentTypeLookup>();
            mockTypeLookup.GetComponentTypeId(typeof(TestStructComponentOne)).Returns(0);
            mockTypeLookup.GetComponentTypeId(typeof(TestStructComponentTwo)).Returns(1);

            var mockComponentPool1 = Substitute.For<IComponentPool<TestStructComponentOne>>();
            mockComponentPool1.OnPoolExtending.Returns(Observable.Empty<bool>());
                
            var mockComponentPool2 = Substitute.For<IComponentPool<TestStructComponentTwo>>();
            mockComponentPool2.OnPoolExtending.Returns(Observable.Empty<bool>());

            var mockComponentDatabase = Substitute.For<IComponentDatabase>();
            mockComponentDatabase.GetPoolFor<TestStructComponentOne>(0).Returns(mockComponentPool1);
            mockComponentDatabase.GetPoolFor<TestStructComponentTwo>(1).Returns(mockComponentPool2);

            var entityAddedSubject = new Subject<IEntity>();
            var entityRemovedSubject = new Subject<IEntity>();
            var mockObservableGroup = Substitute.For<IComputedEntityGroup>();
            mockObservableGroup.OnAdded.Returns(entityAddedSubject);
            mockObservableGroup.OnRemoving.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnRemoved.Returns(entityRemovedSubject);
            
            var mockBatchBuilder = Substitute.For<IBatchBuilder<TestStructComponentOne, TestStructComponentTwo>>();
            
            var batchAccessor = new BatchAccessor<TestStructComponentOne,TestStructComponentTwo>(mockObservableGroup, mockComponentDatabase, mockBatchBuilder, mockTypeLookup);          
            entityAddedSubject.OnNext(null);
            entityRemovedSubject.OnNext(null);
            
            mockBatchBuilder.Received(3).Build(mockObservableGroup.ToArray());
        }
        
        [Fact]
        public void should_update_when_component_pool_changes()
        {
            var mockTypeLookup = Substitute.For<IComponentTypeLookup>();
            mockTypeLookup.GetComponentTypeId(typeof(TestStructComponentOne)).Returns(0);
            mockTypeLookup.GetComponentTypeId(typeof(TestStructComponentTwo)).Returns(1);

            var poolChanging1Subject = new Subject<bool>();
            var mockComponentPool1 = Substitute.For<IComponentPool<TestStructComponentOne>>();
            mockComponentPool1.OnPoolExtending.Returns(poolChanging1Subject);

            var poolChanging2Subject = new Subject<bool>();
            var mockComponentPool2 = Substitute.For<IComponentPool<TestStructComponentTwo>>();
            mockComponentPool2.OnPoolExtending.Returns(poolChanging2Subject);

            var mockComponentDatabase = Substitute.For<IComponentDatabase>();
            mockComponentDatabase.GetPoolFor<TestStructComponentOne>(0).Returns(mockComponentPool1);
            mockComponentDatabase.GetPoolFor<TestStructComponentTwo>(1).Returns(mockComponentPool2);

            var mockObservableGroup = Substitute.For<IComputedEntityGroup>();
            mockObservableGroup.OnAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnRemoving.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnRemoved.Returns(Observable.Empty<IEntity>());
            
            var mockBatchBuilder = Substitute.For<IBatchBuilder<TestStructComponentOne, TestStructComponentTwo>>();
            
            var batchAccessor = new BatchAccessor<TestStructComponentOne,TestStructComponentTwo>(mockObservableGroup, mockComponentDatabase, mockBatchBuilder, mockTypeLookup);          
            poolChanging1Subject.OnNext(false);
            poolChanging2Subject.OnNext(false);
            
            mockBatchBuilder.Received(3).Build(mockObservableGroup.ToArray());
        }
    }
}