﻿using System;
using System.Collections.Generic;
using System.Linq;
using SystemsR3.Events;
using SystemsR3.Executor;
using SystemsR3.Executor.Handlers;
using SystemsR3.Executor.Handlers.Conventional;
using SystemsR3.Pools;
using SystemsR3.Threading;
using EcsR3.Collections;
using EcsR3.Collections.Database;
using EcsR3.Collections.Entity;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using EcsR3.Groups.Observable.Tracking;
using EcsR3.Plugins.Batching.Builders;
using EcsR3.Plugins.Views.Components;
using EcsR3.Plugins.Views.Systems;
using EcsR3.Systems.Handlers;
using EcsR3.Tests.Helpers;
using EcsR3.Tests.Models;
using EcsR3.Tests.Systems;
using EcsR3.Tests.Systems.DeletingScenarios;
using NSubstitute;
using R3;
using SystemsR3.Events.Messages;
using SystemsR3.Scheduling;
using Xunit;
using Xunit.Abstractions;

namespace EcsR3.Tests.Sanity
{
    public class SanityTests
    {
        private ITestOutputHelper _logger;

        public SanityTests(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        private (IObservableGroupManager, IEntityDatabase, IComponentDatabase, IComponentTypeLookup) CreateFramework()
        {
            var componentLookups = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2},
                {typeof(ViewComponent), 3},
                {typeof(TestStructComponentOne), 4},
                {typeof(TestStructComponentTwo), 5},
                {typeof(ComponentWithReactiveProperty), 6}
            };
            var componentLookupType = new ComponentTypeLookup(componentLookups);
            var componentDatabase = new ComponentDatabase(componentLookupType);
            var entityFactory = new DefaultEntityFactory(new IdPool(), componentDatabase, componentLookupType);
            var collectionFactory = new DefaultEntityCollectionFactory(entityFactory);
            var entityDatabase = new EntityDatabase(collectionFactory);
            var groupTrackerFactory = new GroupTrackerFactory(componentLookupType);
            var observableGroupFactory = new DefaultObservableObservableGroupFactory(groupTrackerFactory);
            var observableGroupManager = new ObservableGroupManager(observableGroupFactory, entityDatabase, componentLookupType);

            return (observableGroupManager, entityDatabase, componentDatabase, componentLookupType);
        }

        private SystemExecutor CreateExecutor(IObservableGroupManager observableGroupManager, IUpdateScheduler updateScheduler = null)
        {
            var threadHandler = new DefaultThreadHandler();
            updateScheduler ??= new DefaultUpdateScheduler();
            var reactsToEntityHandler = new ReactToEntitySystemHandler(observableGroupManager);
            var reactsToGroupHandler = new ReactToGroupSystemHandler(observableGroupManager, threadHandler);
            var reactsToDataHandler = new ReactToDataSystemHandler(observableGroupManager);
            var manualSystemHandler = new ManualSystemHandler();
            var setupHandler = new SetupSystemHandler(observableGroupManager);
            var teardownHandler = new TeardownSystemHandler(observableGroupManager);
            var basicEntityHandler = new BasicEntitySystemHandler(observableGroupManager, threadHandler, updateScheduler);

            var conventionalSystems = new List<IConventionalSystemHandler>
            {
                setupHandler,
                reactsToEntityHandler,
                reactsToGroupHandler,
                reactsToDataHandler,
                manualSystemHandler,
                teardownHandler,
                basicEntityHandler
            };

            return new SystemExecutor(conventionalSystems);
        }

        [Fact]
        public void entity_disposal_works_when_using_late_initialized_observable_groups_without_matches()
        {
            var (observableGroupManager, entityDatabase, _, _) = CreateFramework();

            var collection = entityDatabase.GetCollection();
            var entityOne = collection.CreateEntity();
            entityOne.AddComponents(new TestComponentOne(), new TestComponentThree());

            // note, that this behaves differently for entities present before it is called.
            _ = observableGroupManager.GetObservableGroup(new Group(typeof(TestComponentOne), typeof(TestComponentTwo)));

            entityOne.Dispose(); // just asseting this doesn't throw an exception
        }

        [Fact]
        public void should_execute_setup_for_matching_entities()
        {
            var (observableGroupManager, entityDatabase, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);
            executor.AddSystem(new TestSetupSystem());

            var collection = entityDatabase.GetCollection();
            var entityOne = collection.CreateEntity();
            var entityTwo = collection.CreateEntity();

            entityOne.AddComponents(new TestComponentOne(), new TestComponentTwo());
            entityTwo.AddComponents(new TestComponentTwo());

            Assert.Equal("woop", entityOne.GetComponent<TestComponentOne>().Data);
            Assert.Null(entityTwo.GetComponent<TestComponentTwo>().Data);
        }

        [Fact]
        public void should_not_freak_out_when_removing_components_during_removing_event()
        {
            var (observableGroupManager, entityDatabase, _, _) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var entityOne = collection.CreateEntity();

            var timesCalled = 0;
            entityOne.ComponentsRemoved.Subscribe(x =>
            {
                entityOne.RemoveComponent<TestComponentTwo>();
                timesCalled++;
            });

            entityOne.AddComponents(new TestComponentOne(), new TestComponentTwo());
            entityOne.RemoveComponent<TestComponentOne>();

            Assert.Equal(2, timesCalled);
        }

        [Fact]
        public void should_treat_view_handler_as_setup_system_and_teardown_system()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var setupSystemHandler = new SetupSystemHandler(observableGroupManager);
            var teardownSystemHandler = new TeardownSystemHandler(observableGroupManager);

            var viewSystem = Substitute.For<IViewResolverSystem>();

            Assert.True(setupSystemHandler.CanHandleSystem(viewSystem));
            Assert.True(teardownSystemHandler.CanHandleSystem(viewSystem));
        }

        [Fact]
        public void should_trigger_both_setup_and_teardown_for_view_resolver()
        {
            var (observableGroupManager, entityDatabase, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);
            var viewResolverSystem = new TestViewResolverSystem(new EventSystem(new MessageBroker(), new DefaultThreadHandler()),
                new Group(typeof(TestComponentOne), typeof(ViewComponent)));
            executor.AddSystem(viewResolverSystem);

            var setupCalled = false;
            viewResolverSystem.OnSetup = entity => { setupCalled = true; };
            var teardownCalled = false;
            viewResolverSystem.OnTeardown = entity => { teardownCalled = true; };

            var collection = entityDatabase.GetCollection();
            var entityOne = collection.CreateEntity();
            entityOne.AddComponents(new TestComponentOne(), new ViewComponent());

            collection.RemoveEntity(entityOne.Id);

            Assert.True(setupCalled);
            Assert.True(teardownCalled);
        }

        [Fact]
        public void should_listen_to_multiple_collections_for_updates()
        {
            var (observableGroupManager, entityDatabase, _, _) = CreateFramework();

            var group = new Group(typeof(TestComponentOne));
            var collection1 = entityDatabase.CreateCollection(1);
            var collection2 = entityDatabase.CreateCollection(2);

            var addedTimesCalled = 0;
            var removingTimesCalled = 0;
            var removedTimesCalled = 0;
            var observableGroup = observableGroupManager.GetObservableGroup(group, 1, 2);
            observableGroup.OnEntityAdded.Subscribe(x => addedTimesCalled++);
            observableGroup.OnEntityRemoving.Subscribe(x => removingTimesCalled++);
            observableGroup.OnEntityRemoved.Subscribe(x => removedTimesCalled++);

            var entity1 = collection1.CreateEntity();
            entity1.AddComponent<TestComponentOne>();

            var entity2 = collection2.CreateEntity();
            entity2.AddComponent<TestComponentOne>();

            collection1.RemoveEntity(entity1.Id);
            collection2.RemoveEntity(entity2.Id);

            Assert.Equal(2, addedTimesCalled);
            Assert.Equal(2, removingTimesCalled);
            Assert.Equal(2, removedTimesCalled);
        }

        [Fact]
        public unsafe void should_keep_state_with_batches()
        {
            var (_, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection1 = entityDatabase.CreateCollection(1);
            var entity1 = collection1.CreateEntity();

            var startingInt = 2;
            var finalInt = 10;
            var startingFloat = 5.0f;
            var finalFloat = 20.0f;

            ref var structComponent1 = ref entity1.AddComponent<TestStructComponentOne>(4);
            var component1Allocation = entity1.ComponentAllocations[4];
            structComponent1.Data = startingInt;

            ref var structComponent2 = ref entity1.AddComponent<TestStructComponentTwo>(5);
            var component2Allocation = entity1.ComponentAllocations[5];
            structComponent2.Data = startingFloat;

            var entities = new[] {entity1};
            var batchBuilder = new BatchBuilder<TestStructComponentOne, TestStructComponentTwo>(componentDatabase, componentLookup);
            var batch = batchBuilder.Build(entities);

            ref var initialBatchData = ref batch.Batches[0];
            ref var component1 = ref *initialBatchData.Component1;
            ref var component2 = ref *initialBatchData.Component2;
            Assert.Equal(startingInt, component1.Data);
            Assert.Equal(startingFloat, component2.Data);

            component1.Data = finalInt;
            component2.Data = finalFloat;

            Assert.Equal(finalInt, (*batch.Batches[0].Component1).Data);
            Assert.Equal(finalInt, structComponent1.Data);
            Assert.Equal(finalInt, componentDatabase.Get<TestStructComponentOne>(4, component1Allocation).Data);
            Assert.Equal(finalFloat, (*batch.Batches[0].Component2).Data);
            Assert.Equal(finalFloat, structComponent2.Data);
            Assert.Equal(finalFloat, componentDatabase.Get<TestStructComponentTwo>(5, component2Allocation).Data);
        }

        [Fact]
        public unsafe void should_retain_pointer_through_new_struct()
        {
            var (_, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection1 = entityDatabase.CreateCollection(1);
            var entity1 = collection1.CreateEntity();

            var startingInt = 2;
            var finalInt = 10;
            var startingFloat = 5.0f;
            var finalFloat = 20.0f;

            ref var structComponent1 = ref entity1.AddComponent<TestStructComponentOne>(4);
            var component1Allocation = entity1.ComponentAllocations[4];
            structComponent1.Data = startingInt;

            ref var structComponent2 = ref entity1.AddComponent<TestStructComponentTwo>(5);
            var component2Allocation = entity1.ComponentAllocations[5];
            structComponent2.Data = startingFloat;

            var entities = new[] {entity1};
            var batchBuilder = new BatchBuilder<TestStructComponentOne, TestStructComponentTwo>(componentDatabase, componentLookup);
            var batch = batchBuilder.Build(entities);

            ref var initialBatchData = ref batch.Batches[0];
            ref var component1 = ref *initialBatchData.Component1;
            ref var component2 = ref *initialBatchData.Component2;

            Assert.Equal(startingInt, component1.Data);
            Assert.Equal(startingFloat, component2.Data);

            component1 = new TestStructComponentOne {Data = finalInt};
            component2 = new TestStructComponentTwo {Data = finalFloat};

            Assert.Equal(finalInt, (*batch.Batches[0].Component1).Data);
            Assert.Equal(finalInt, structComponent1.Data);
            Assert.Equal(finalInt, componentDatabase.Get<TestStructComponentOne>(4, component1Allocation).Data);
            Assert.Equal(finalFloat, (*batch.Batches[0].Component2).Data);
            Assert.Equal(finalFloat, structComponent2.Data);
            Assert.Equal(finalFloat, componentDatabase.Get<TestStructComponentTwo>(5, component2Allocation).Data);
        }

        [Fact]
        public void should_allocate_entities_correctly()
        {
            var expectedSize = 5000;
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var observableGroup = observableGroupManager.GetObservableGroup(new Group(typeof(ViewComponent), typeof(TestComponentOne)));

            for (var i = 0; i < expectedSize; i++)
            {
                var entity = collection.CreateEntity();
                entity.AddComponents(new ViewComponent(), new TestComponentOne());
            }

            Assert.Equal(expectedSize, collection.Count);
            Assert.Equal(expectedSize, observableGroup.Count);

            var viewComponentPool = componentDatabase.GetPoolFor<ViewComponent>(componentLookup.GetComponentTypeId(typeof(ViewComponent)));
            Assert.Equal(expectedSize, viewComponentPool.Components.Length);

            var testComponentPool = componentDatabase.GetPoolFor<TestComponentOne>(componentLookup.GetComponentTypeId(typeof(TestComponentOne)));
            Assert.Equal(expectedSize, testComponentPool.Components.Length);
        }
        
        [Fact(Skip = "This wont work due to how R3 handles disposing, need to look at in more depth")]
        public void should_handle_deletion_while_setting_up_in_reactive_data_systems()
        {
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var executor = CreateExecutor(observableGroupManager);

            var entity = collection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingReactiveDataTestSystem1(collection);
            var systemB = new DeletingReactiveDataTestSystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);

            var reactiveDataSystemsHandler = (ReactToDataSystemHandler)executor._conventionalSystemHandlers.Single(x => x is ReactToDataSystemHandler);
            Assert.Equal(2, reactiveDataSystemsHandler.EntitySubscriptions.Count);
            Assert.Empty(reactiveDataSystemsHandler.EntitySubscriptions[systemA].Values);
            Assert.Empty(reactiveDataSystemsHandler.EntitySubscriptions[systemB].Values);
        }
        
        [Fact(Skip = "This wont work due to how R3 handles disposing, need to look at in more depth")]
        public void should_handle_deletion_while_setting_up_in_reactive_entity_systems()
        {
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var executor = CreateExecutor(observableGroupManager);

            var entity = collection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingReactiveEntityTestSystem1(collection);
            var systemB = new DeletingReactiveEntityTestSystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);

            var systemHandler = (ReactToEntitySystemHandler)executor._conventionalSystemHandlers.Single(x => x is ReactToEntitySystemHandler);
            Assert.Equal(2, systemHandler._entitySubscriptions.Count);
            Assert.Empty(systemHandler._entitySubscriptions[systemA].Values);
            Assert.Empty(systemHandler._entitySubscriptions[systemB].Values);
        }
        
        [Fact]
        public void should_handle_deletion_while_setting_up_in_setup_systems()
        {
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var executor = CreateExecutor(observableGroupManager);

            var entity = collection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingSetupTestSystem1(collection);
            var systemB = new DeletingSetupTestSystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);

            var reactiveDataSystemsHandler = (SetupSystemHandler)executor._conventionalSystemHandlers.Single(x => x is SetupSystemHandler);
            Assert.Equal(2, reactiveDataSystemsHandler._entitySubscriptions.Count);
            Assert.Empty(reactiveDataSystemsHandler._entitySubscriptions[systemA].Values);
            Assert.Empty(reactiveDataSystemsHandler._entitySubscriptions[systemB].Values);
        }

        [Fact]
        public void should_listen_for_removals_on_basic_entity_systems()
        {
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);

            var entity = collection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingBasicEntitySystem1(collection);
            var systemB = new DeletingBasicEntitySystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);
            
            updateTrigger.OnNext(new ElapsedTime());
        }
        
        [Fact(Skip = "This wont work due to how R3 handles disposing, need to look at in more depth")]
        public void should_handle_removals_during_add_between_different_system_types()
        {
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);

            var entity = collection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingReactiveDataTestSystem1(collection);
            var systemB = new DeletingBasicEntitySystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);
            
            updateTrigger.OnNext(new ElapsedTime());
        }
        
        [Fact]
        public void should_handle_removals_during_add_phase_across_two_systems()
        {
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);
            
            var systemA = new DeletingReactiveDataTestSystem1(collection);
            var systemB = new DeletingBasicEntitySystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);
            
            var entity = collection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());
            
            updateTrigger.OnNext(new ElapsedTime());
        }
        
        [Fact]
        public void should_handle_removals_during_add_phase_across_multiple_systems()
        {
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);
            
            var system1 = new DeletingReactiveDataTestSystem1(collection);
            var system2 = new DeletingReactiveDataTestSystem2();
            var system3 = new DeletingReactiveEntityTestSystem1(collection);
            var system4 = new DeletingReactiveEntityTestSystem2();
            var system5 = new DeletingSetupTestSystem1(collection);
            var system6 = new DeletingSetupTestSystem2();
            var system7 = new DeletingBasicEntitySystem1(collection);
            var system8 = new DeletingBasicEntitySystem2();
            executor.AddSystem(system1);
            executor.AddSystem(system2);
            executor.AddSystem(system3);
            executor.AddSystem(system4);
            executor.AddSystem(system5);
            executor.AddSystem(system6);
            executor.AddSystem(system7);
            executor.AddSystem(system8);
            
            var entity = collection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());
            
            updateTrigger.OnNext(new ElapsedTime());
        }
        
        [Fact]
        public void should_handle_removals_during_add_phase_across_multiple_systems_with_overlapping_groups()
        {
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);
            
            var system1 = new DeletingReactiveDataTestSystem1(collection);
            var system2 = new DeletingReactiveDataTestSystem2();
            var system3 = new DeletingOverlappingReactiveEntityTestSystem1(collection);
            var system4 = new DeletingOverlappingReactiveEntityTestSystem2();
            var system5 = new DeletingOverlappingSetupTestSystem1(collection);
            var system6 = new DeletingOverlappingSetupTestSystem2();
            var system7 = new DeletingOverlappingBasicEntitySystem1(collection);
            var system8 = new DeletingOverlappingBasicEntitySystem2();
            executor.AddSystem(system1);
            executor.AddSystem(system2);
            executor.AddSystem(system3);
            executor.AddSystem(system4);
            executor.AddSystem(system5);
            executor.AddSystem(system6);
            executor.AddSystem(system7);
            executor.AddSystem(system8);
            
            var entity1 = collection.CreateEntity();
            entity1.AddComponents(new ComponentWithReactiveProperty(), new TestComponentOne());
            var entity2 = collection.CreateEntity();
            entity2.AddComponents(new ComponentWithReactiveProperty(), new TestComponentTwo());
            var entity3 = collection.CreateEntity();
            entity3.AddComponents(new ComponentWithReactiveProperty(), new TestComponentThree());
            
            updateTrigger.OnNext(new ElapsedTime());
        }
    }
}