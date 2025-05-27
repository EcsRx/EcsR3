using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcsR3.Collections.Entities;
using EcsR3.Collections.Entities.Pools;
using SystemsR3.Events;
using SystemsR3.Executor;
using SystemsR3.Executor.Handlers;
using SystemsR3.Executor.Handlers.Conventional;
using SystemsR3.Pools;
using SystemsR3.Threading;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Computeds.Entities.Factories;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Entities.Routing;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Groups.Tracking;
using EcsR3.Plugins.Views.Components;
using EcsR3.Plugins.Views.Systems;
using EcsR3.Systems.Batching.Convention;
using EcsR3.Systems.Handlers;
using EcsR3.Tests.Helpers;
using EcsR3.Tests.Models;
using EcsR3.Tests.Systems;
using EcsR3.Tests.Systems.DeletingScenarios;
using NSubstitute;
using R3;
using SystemsR3.Events.Messages;
using SystemsR3.Scheduling;
using SystemsR3.Systems;
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

        private (IComputedEntityGroupRegistry, IEntityCollection, IComponentDatabase, IComponentTypeLookup, IEntityChangeRouter, IComputedComponentGroupRegistry, IEntityComponentAccessor) CreateFramework()
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

            var componentDatabaseConfig = new ComponentDatabaseConfig(100, 100);
            var componentLookupType = new ComponentTypeLookup(componentLookups);
            var componentDatabase = new ComponentDatabase(componentLookupType, componentDatabaseConfig);
            var entityChangeRouter = new EntityChangeRouter(componentLookupType);
            var entityIdPool = new EntityIdPool();
            var entityAllocationDatabase = new EntityAllocationDatabase(entityIdPool, componentDatabase, entityChangeRouter, componentLookupType);;
            var entityComponentAccessor = new EntityComponentAccessor(componentLookupType, entityAllocationDatabase, componentDatabase, entityChangeRouter);
            var entityCollection = new EntityCollection(entityAllocationDatabase, entityComponentAccessor);
            var groupTrackerFactory = new GroupTrackerFactory(entityChangeRouter, entityAllocationDatabase);
            var observableGroupFactory = new ComputedEntityGroupFactory(groupTrackerFactory, entityCollection);
            var observableGroupManager = new ComputedEntityGroupRegistry(observableGroupFactory, entityCollection, componentLookupType);
            var computedComponentGroupRegistry = new ComputedComponentGroupRegistry(observableGroupManager, componentLookupType);

            return (observableGroupManager, entityCollection, componentDatabase, componentLookupType, entityChangeRouter, computedComponentGroupRegistry, entityComponentAccessor);
        }

        private SystemExecutor CreateExecutor(IComputedEntityGroupRegistry observableEntityGroupRegistry, IUpdateScheduler updateScheduler = null)
        {
            var threadHandler = new DefaultThreadHandler();
            updateScheduler ??= new DefaultUpdateScheduler();
            var reactsToEntityHandler = new ReactToEntitySystemHandler(observableEntityGroupRegistry);
            var reactsToGroupHandler = new ReactToGroupSystemHandler(observableEntityGroupRegistry, threadHandler);
            var reactsToDataHandler = new ReactToDataSystemHandler(observableEntityGroupRegistry);
            var manualSystemHandler = new ManualSystemHandler();
            var setupHandler = new SetupSystemHandler(observableEntityGroupRegistry);
            var teardownHandler = new TeardownSystemHandler(observableEntityGroupRegistry);
            var basicEntityHandler = new BasicEntitySystemHandler(observableEntityGroupRegistry, threadHandler, updateScheduler);

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
            var (observableGroupManager, entityCollection, _, _, _, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);
            executor.AddSystem(new TestSetupSystem());

            var entityOne = entityCollection.Create();
            var entityTwo = entityCollection.Create();

            entityOne.AddComponents(new TestComponentOne(), new TestComponentTwo());
            entityTwo.AddComponents(new TestComponentTwo());

            Assert.Equal("woop", entityOne.GetComponent<TestComponentOne>().Data);
            Assert.Null(entityTwo.GetComponent<TestComponentTwo>().Data);
        }

        [Fact]
        public void should_not_freak_out_when_removing_components_during_removing_event()
        {
            var (observableGroupManager, entityCollection, _, componentTypeLookup, entityChangeRouter, _, _) = CreateFramework();
            var entityOne = entityCollection.Create();

            var testComponentOneTypeId = componentTypeLookup.GetComponentTypeId(typeof(TestComponentOne));
            var testComponentTwoTypeId = componentTypeLookup.GetComponentTypeId(typeof(TestComponentTwo));
            
            var componentOneInvocations = new List<int>();
            entityChangeRouter
                .OnEntityRemovedComponents(new []{testComponentOneTypeId})
                .Subscribe(x => {
                    entityOne.RemoveComponent<TestComponentTwo>();
                    componentOneInvocations.Add(x.EntityId);
                });
            
            var componentTwoInvocations = new List<int>();
            entityChangeRouter
                .OnEntityRemovedComponents(new []{testComponentTwoTypeId})
                .Subscribe(x => componentTwoInvocations.Add(x.EntityId));

            entityOne.AddComponents(new TestComponentOne(), new TestComponentTwo());
            entityOne.RemoveComponent<TestComponentOne>();

            Thread.Sleep(100);
            
            Assert.NotEmpty(componentOneInvocations);
            Assert.Single(componentOneInvocations, entityOne.Id);
            Assert.NotEmpty(componentTwoInvocations);
            Assert.Single(componentTwoInvocations, entityOne.Id);
        }

        [Fact]
        public void should_treat_view_handler_as_setup_system_and_teardown_system()
        {
            var observableGroupManager = Substitute.For<IComputedEntityGroupRegistry>();
            var setupSystemHandler = new SetupSystemHandler(observableGroupManager);
            var teardownSystemHandler = new TeardownSystemHandler(observableGroupManager);

            var viewSystem = Substitute.For<IViewResolverSystem>();

            Assert.True(setupSystemHandler.CanHandleSystem(viewSystem));
            Assert.True(teardownSystemHandler.CanHandleSystem(viewSystem));
        }

        [Fact]
        public void should_trigger_both_setup_and_teardown_for_view_resolver()
        {
            var (observableGroupManager, entityCollection, _, _, _, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);
            var viewResolverSystem = new TestViewResolverSystem(new EventSystem(new MessageBroker(), new DefaultThreadHandler()),
                new Group(typeof(TestComponentOne), typeof(ViewComponent)));
            executor.AddSystem(viewResolverSystem);

            var setupCalled = false;
            viewResolverSystem.OnSetup = entity => { setupCalled = true; };
            var teardownCalled = false;
            viewResolverSystem.OnTeardown = entity => { teardownCalled = true; };

            var entityOne = entityCollection.Create();
            entityOne.AddComponents(new TestComponentOne(), new ViewComponent());

            entityCollection.Remove(entityOne.Id);

            Assert.True(setupCalled);
            Assert.True(teardownCalled);
        }
        
        [Fact]
        public void should_call_setup_system_before_setup_and_teardown_for_entities_on_view_system()
        {
            var (observableGroupManager, entityCollection, _, _, _, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);

            var expectedCallList = new[] { "start-system", "setup", "teardown", "stop-system" };
            var actualCallList = new List<string>();
            
            var viewResolverSystem = new HybridSetupSystem(actualCallList.Add, new Group(typeof(TestComponentOne), typeof(ViewComponent)));
            var entityOne = entityCollection.Create();
            entityOne.AddComponents(new TestComponentOne(), new ViewComponent());
            
            executor.AddSystem(viewResolverSystem);
            entityCollection.Remove(entityOne.Id);
            executor.RemoveSystem(viewResolverSystem);

            Assert.Equal(expectedCallList, actualCallList);
        }

        [Fact]
        public void should_allocate_entities_correctly()
        {
            var expectedSize = 5000;
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _, _, _) = CreateFramework();
            var observableGroup = observableGroupManager.GetComputedGroup(new Group(typeof(ViewComponent), typeof(TestComponentOne)));

            for (var i = 0; i < expectedSize; i++)
            {
                var entity = entityCollection.Create();
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
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);

            var entity = entityCollection.Create();
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
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);

            var entity = entityCollection.Create();
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
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);

            var entity = entityCollection.Create();
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
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _, _, _) = CreateFramework();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);

            var entity = entityCollection.Create();
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
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _, _, _) = CreateFramework();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);

            var entity = entityCollection.Create();
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
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _, _, _) = CreateFramework();
            var updateTrigger = new Subject<ElapsedTime>();
            var updateScheduler = new ManualUpdateScheduler(updateTrigger);
            var executor = CreateExecutor(observableGroupManager, updateScheduler);
            
            var systemA = new DeletingReactiveDataTestSystem1(entityCollection);
            var systemB = new DeletingBasicEntitySystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);
            
            var entity = entityCollection.Create();
            entity.AddComponent(new ComponentWithReactiveProperty());
            
            updateTrigger.OnNext(new ElapsedTime());
        }
        
        [Fact]
        public void should_handle_removals_during_add_phase_across_multiple_systems()
        {
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _, _, _) = CreateFramework();
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
            
            var entity = entityCollection.Create();
            entity.AddComponent(new ComponentWithReactiveProperty());
            
            updateTrigger.OnNext(new ElapsedTime());
        }
        
        [Fact]
        public void should_handle_removals_during_add_phase_across_multiple_systems_with_overlapping_groups()
        {
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _, _, _) = CreateFramework();
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
            
            var entity1 = entityCollection.Create();
            entity1.AddComponents(new ComponentWithReactiveProperty(), new TestComponentOne());
            var entity2 = entityCollection.Create();
            entity2.AddComponents(new ComponentWithReactiveProperty(), new TestComponentTwo());
            var entity3 = entityCollection.Create();
            entity3.AddComponents(new ComponentWithReactiveProperty(), new TestComponentThree());
            
            updateTrigger.OnNext(new ElapsedTime());
        }

        [Fact]
        public void should_call_batched_process_correct_number_of_times()
        {
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _, computedComponentGroupRegistry, _) = CreateFramework();
            
            var entityCount = 1000;
            var entities = entityCollection.CreateMany<BatchedProcessTestSystem.BatchedProcessTestBlueprint>(entityCount);

            var timesToProcess = 100;
            var batchedTestSystem = new BatchedProcessTestSystem(componentDatabase, computedComponentGroupRegistry, new DefaultThreadHandler());
            batchedTestSystem.StartSystem();
            
            for (var i = 0; i < timesToProcess; i++)
            { batchedTestSystem.ForceProcess(); }
            
            var expectedCalls = entityCount * timesToProcess;
            Assert.Equal(expectedCalls, batchedTestSystem.TimesCalled);
        }
        
        [Fact]
        public void should_call_batched_process_correct_number_of_times_with_multithreading()
        {
            var (observableGroupManager, entityCollection, componentDatabase, componentLookup, _, computedComponentGroupRegistry, _) = CreateFramework();
            
            var entityCount = 1000;
            var entities = entityCollection.CreateMany<BatchedProcessTestSystem.BatchedProcessTestBlueprint>(entityCount);

            var timesToProcess = 100;
            var batchedTestSystem = new MultithreadedBatchedProcessTestSystem(componentDatabase, computedComponentGroupRegistry, new DefaultThreadHandler());
            batchedTestSystem.StartSystem();
            
            for (var i = 0; i < timesToProcess; i++)
            { batchedTestSystem.ForceProcess(); }
            
            var expectedCalls = entityCount * timesToProcess;
            Assert.Equal(expectedCalls, batchedTestSystem.TimesCalled);
        }

        [Fact]
        public void should_correctly_loop_all_elements_with_parallel_for()
        {
            var elementCount = 1000;
            var timesCalled = 0;
            var _lock = new object();
            
            Parallel.For(0, elementCount, i =>
            {
                lock (_lock)
                {
                    timesCalled++;
                }
            });

            Assert.Equal(elementCount, timesCalled);
        }
    }
}