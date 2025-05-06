using System;
using System.Collections.Generic;
using System.Linq;
using SystemsR3.Events;
using SystemsR3.Executor;
using SystemsR3.Executor.Handlers;
using SystemsR3.Executor.Handlers.Conventional;
using SystemsR3.Pools;
using SystemsR3.Threading;
using EcsR3.Collections;
using EcsR3.Collections.Entity;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Entities.Routing;
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
/*
namespace EcsR3.Tests.Sanity
{
    public class SanityTests
    {
        private ITestOutputHelper _logger;

        public SanityTests(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        private (IObservableGroupManager, IEntityCollection, IComponentDatabase, IComponentTypeLookup, IEntityChangeRouter) CreateFramework()
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
            var entityChangeRouter = new EntityChangeRouter(componentLookupType);
            var entityFactory = new DefaultEntityFactory(new IdPool(), componentDatabase, componentLookupType, entityChangeRouter);
            var entityCollection = new EntityCollection(entityFactory);
            var groupTrackerFactory = new GroupTrackerFactory(entityChangeRouter);
            var observableGroupFactory = new DefaultObservableObservableGroupFactory(groupTrackerFactory, entityCollection);
            var observableGroupManager = new ObservableGroupManager(observableGroupFactory, entityCollection, componentLookupType);

            return (observableGroupManager, entityCollection, componentDatabase, componentLookupType, entityChangeRouter);
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
        public void should_execute_setup_for_matching_entities()
        {
            var (observableGroupManager, entityCollection, _, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);
            executor.AddSystem(new TestSetupSystem());

            var entityOne = entityCollection.CreateEntity();
            var entityTwo = entityCollection.CreateEntity();

            entityOne.AddComponents(new TestComponentOne(), new TestComponentTwo());
            entityTwo.AddComponents(new TestComponentTwo());

            Assert.Equal("woop", entityOne.GetComponent<TestComponentOne>().Data);
            Assert.Null(entityTwo.GetComponent<TestComponentTwo>().Data);
        }

        [Fact]
        public void should_not_freak_out_when_removing_components_during_removing_event()
        {
            var (observableGroupManager, entityCollection, _, componentTypeLookup, entityChangeRouter) = CreateFramework();
            var entityOne = entityCollection.CreateEntity();

            var testComponentTwoTypeId = componentTypeLookup.GetComponentTypeId(typeof(TestComponentTwo));
            var timesCalled = 0;
            entityChangeRouter.SubscribeOnEntityRemovedComponent(testComponentTwoTypeId).Subscribe(x => {
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
            var (observableGroupManager, entityCollection, _, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);
            var viewResolverSystem = new TestViewResolverSystem(new EventSystem(new MessageBroker(), new DefaultThreadHandler()),
                new Group(typeof(TestComponentOne), typeof(ViewComponent)));
            executor.AddSystem(viewResolverSystem);

            var setupCalled = false;
            viewResolverSystem.OnSetup = entity => { setupCalled = true; };
            var teardownCalled = false;
            viewResolverSystem.OnTeardown = entity => { teardownCalled = true; };

            var entityOne = entityCollection.CreateEntity();
            entityOne.AddComponents(new TestComponentOne(), new ViewComponent());

            entityCollection.RemoveEntity(entityOne.Id);

            Assert.True(setupCalled);
            Assert.True(teardownCalled);
        }
        
        [Fact]
        public void should_call_setup_system_before_setup_and_teardown_for_entities_on_view_system()
        {
            var (observableGroupManager, entityCollection, _, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);

            var expectedCallList = new[] { "start-system", "setup", "teardown", "stop-system" };
            var actualCallList = new List<string>();
            
            var viewResolverSystem = new HybridSetupSystem(actualCallList.Add, new Group(typeof(TestComponentOne), typeof(ViewComponent)));
            var entityOne = entityCollection.CreateEntity();
            entityOne.AddComponents(new TestComponentOne(), new ViewComponent());
            
            executor.AddSystem(viewResolverSystem);
            entityCollection.RemoveEntity(entityOne.Id);
            executor.RemoveSystem(viewResolverSystem);

            Assert.Equal(expectedCallList, actualCallList);
        }

        [Fact]
        public unsafe void should_keep_state_with_batches()
        {
            var (_, entityCollection, componentDatabase, componentLookup, _) = CreateFramework();
            var entity1 = entityCollection.CreateEntity();

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
            var (_, entityCollection, componentDatabase, componentLookup, _) = CreateFramework();
            var entity1 = entityCollection.CreateEntity();

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
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _) = CreateFramework();
            var observableGroup = observableGroupManager.GetObservableGroup(new Group(typeof(ViewComponent), typeof(TestComponentOne)));

            for (var i = 0; i < expectedSize; i++)
            {
                var entity = entityCollection.CreateEntity();
                entity.AddComponents(new ViewComponent(), new TestComponentOne());
            }

            Assert.Equal(expectedSize, entityCollection.Count);
            Assert.Equal(expectedSize, observableGroup.Count);

            var viewComponentPool = componentDatabase.GetPoolFor<ViewComponent>(componentLookup.GetComponentTypeId(typeof(ViewComponent)));
            Assert.Equal(expectedSize, viewComponentPool.Components.Length);

            var testComponentPool = componentDatabase.GetPoolFor<TestComponentOne>(componentLookup.GetComponentTypeId(typeof(TestComponentOne)));
            Assert.Equal(expectedSize, testComponentPool.Components.Length);
        }
        
        [Fact(Skip = "This wont work due to how R3 handles disposing, need to look at in more depth")]
        public void should_handle_deletion_while_setting_up_in_reactive_data_systems()
        {
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);

            var entity = entityCollection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingReactiveDataTestSystem1(entityCollection);
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
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);

            var entity = entityCollection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingReactiveEntityTestSystem1(entityCollection);
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
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);

            var entity = entityCollection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingSetupTestSystem1(entityCollection);
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
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _) = CreateFramework();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);

            var entity = entityCollection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingBasicEntitySystem1(entityCollection);
            var systemB = new DeletingBasicEntitySystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);
            
            updateTrigger.OnNext(new ElapsedTime());
        }
        
        [Fact(Skip = "This wont work due to how R3 handles disposing, need to look at in more depth")]
        public void should_handle_removals_during_add_between_different_system_types()
        {
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _) = CreateFramework();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);

            var entity = entityCollection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingReactiveDataTestSystem1(entityCollection);
            var systemB = new DeletingBasicEntitySystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);
            
            updateTrigger.OnNext(new ElapsedTime());
        }
        
        [Fact]
        public void should_handle_removals_during_add_phase_across_two_systems()
        {
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _) = CreateFramework();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);
            
            var systemA = new DeletingReactiveDataTestSystem1(entityCollection);
            var systemB = new DeletingBasicEntitySystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);
            
            var entity = entityCollection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());
            
            updateTrigger.OnNext(new ElapsedTime());
        }
        
        [Fact]
        public void should_handle_removals_during_add_phase_across_multiple_systems()
        {
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _) = CreateFramework();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);
            
            var system1 = new DeletingReactiveDataTestSystem1(entityCollection);
            var system2 = new DeletingReactiveDataTestSystem2();
            var system3 = new DeletingReactiveEntityTestSystem1(entityCollection);
            var system4 = new DeletingReactiveEntityTestSystem2();
            var system5 = new DeletingSetupTestSystem1(entityCollection);
            var system6 = new DeletingSetupTestSystem2();
            var system7 = new DeletingBasicEntitySystem1(entityCollection);
            var system8 = new DeletingBasicEntitySystem2();
            executor.AddSystem(system1);
            executor.AddSystem(system2);
            executor.AddSystem(system3);
            executor.AddSystem(system4);
            executor.AddSystem(system5);
            executor.AddSystem(system6);
            executor.AddSystem(system7);
            executor.AddSystem(system8);
            
            var entity = entityCollection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());
            
            updateTrigger.OnNext(new ElapsedTime());
        }
        
        [Fact]
        public void should_handle_removals_during_add_phase_across_multiple_systems_with_overlapping_groups()
        {
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _) = CreateFramework();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);
            
            var system1 = new DeletingReactiveDataTestSystem1(entityCollection);
            var system2 = new DeletingReactiveDataTestSystem2();
            var system3 = new DeletingOverlappingReactiveEntityTestSystem1(entityCollection);
            var system4 = new DeletingOverlappingReactiveEntityTestSystem2();
            var system5 = new DeletingOverlappingSetupTestSystem1(entityCollection);
            var system6 = new DeletingOverlappingSetupTestSystem2();
            var system7 = new DeletingOverlappingBasicEntitySystem1(entityCollection);
            var system8 = new DeletingOverlappingBasicEntitySystem2();
            executor.AddSystem(system1);
            executor.AddSystem(system2);
            executor.AddSystem(system3);
            executor.AddSystem(system4);
            executor.AddSystem(system5);
            executor.AddSystem(system6);
            executor.AddSystem(system7);
            executor.AddSystem(system8);
            
            var entity1 = entityCollection.CreateEntity();
            entity1.AddComponents(new ComponentWithReactiveProperty(), new TestComponentOne());
            var entity2 = entityCollection.CreateEntity();
            entity2.AddComponents(new ComponentWithReactiveProperty(), new TestComponentTwo());
            var entity3 = entityCollection.CreateEntity();
            entity3.AddComponents(new ComponentWithReactiveProperty(), new TestComponentThree());
            
            updateTrigger.OnNext(new ElapsedTime());
        }
    }
}
*/