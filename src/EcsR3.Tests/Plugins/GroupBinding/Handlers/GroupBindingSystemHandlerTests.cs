using SystemsR3.Systems;
using SystemsR3.Systems.Conventional;
using EcsR3.Collections;
using EcsR3.Groups;
using EcsR3.Plugins.GroupBinding.Exceptions;
using EcsR3.Plugins.GroupBinding.Systems.Handlers;
using EcsR3.Systems;
using EcsR3.Tests.Models;
using EcsR3.Tests.Plugins.GroupBinding.Handlers.Helpers;
using NSubstitute;
using Xunit;

namespace EcsR3.Tests.Plugins.GroupBinding.Handlers
{
    public class GroupBindingSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var reactToEntitySystemHandler = new GroupBindingSystemHandler(observableGroupManager);
            
            var fakeMatchingSystem1 = Substitute.For<ISetupSystem>();
            var fakeMatchingSystem2 = Substitute.For<IReactToEntitySystem>();
            var fakeMatchingSystem3 = Substitute.For<IGroupSystem>();
            var fakeMatchingSystem4 = Substitute.For<IManualSystem>();
            var fakeMatchingSystem5 = Substitute.For<ISystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem1));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem2));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem3));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem4));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem5));
        }
        
        [Fact]
        public void should_correctly_get_applicable_properties()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var reactToEntitySystemHandler = new GroupBindingSystemHandler(observableGroupManager);
            var dummySystem = new SystemWithAutoGroupPopulation();

            var applicableProperties = reactToEntitySystemHandler.GetApplicableProperties(dummySystem.GetType());
            Assert.Equal(3, applicableProperties.Length);
            Assert.Contains(applicableProperties, x => x.Name == nameof(dummySystem.ObservableGroupA));
            Assert.Contains(applicableProperties, x => x.Name == nameof(dummySystem.ObservableGroupB));
            Assert.Contains(applicableProperties, x => x.Name == nameof(dummySystem.ObservableGroupCInCollection7));
        }

        [Fact]
        public void should_correctly_get_applicable_fields()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var reactToEntitySystemHandler = new GroupBindingSystemHandler(observableGroupManager);
            var dummySystem = new SystemWithAutoGroupPopulation();

            var applicableProperties = reactToEntitySystemHandler.GetApplicableFields(dummySystem.GetType());
            Assert.Equal(3, applicableProperties.Length);
            Assert.Contains(applicableProperties, x => x.Name == nameof(dummySystem.ObservableGroupC));
            Assert.Contains(applicableProperties, x => x.Name == nameof(dummySystem.ObservableGroupAInCollection2));
            Assert.Contains(applicableProperties, x => x.Name == nameof(dummySystem.ObservableGroupBInCollection5));
        }
        
        [Fact]
        public void should_get_group_from_members()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var reactToEntitySystemHandler = new GroupBindingSystemHandler(observableGroupManager);
            var dummySystem = new SystemWithAutoGroupPopulation();

            var member1 = dummySystem.GetType().GetProperty(nameof(dummySystem.ObservableGroupA));
            var member2 = dummySystem.GetType().GetProperty(nameof(dummySystem.ObservableGroupB));
            var member3 = dummySystem.GetType().GetField(nameof(dummySystem.ObservableGroupC));
            var member4 = dummySystem.GetType().GetField(nameof(dummySystem.ObservableGroupAInCollection2));
            var member5 = dummySystem.GetType().GetField(nameof(dummySystem.ObservableGroupBInCollection5));
            var member6 = dummySystem.GetType().GetProperty(nameof(dummySystem.ObservableGroupCInCollection7));

            var groupWithAffinity1 = reactToEntitySystemHandler.GetGroupFromAttributeIfAvailable(dummySystem, member1);
            var groupWithAffinity2 = reactToEntitySystemHandler.GetGroupFromAttributeIfAvailable(dummySystem, member2);
            var groupWithAffinity3 = reactToEntitySystemHandler.GetGroupFromAttributeIfAvailable(dummySystem, member3);
            var groupWithAffinity4 = reactToEntitySystemHandler.GetGroupFromAttributeIfAvailable(dummySystem, member4);
            var groupWithAffinity5 = reactToEntitySystemHandler.GetGroupFromAttributeIfAvailable(dummySystem, member5);
            var groupWithAffinity6 = reactToEntitySystemHandler.GetGroupFromAttributeIfAvailable(dummySystem, member6);

            Assert.Single(groupWithAffinity1.RequiredComponents);
            Assert.Contains(groupWithAffinity1.RequiredComponents, x => x == typeof(TestComponentOne));

            Assert.Single(groupWithAffinity2.RequiredComponents);
            Assert.Contains(groupWithAffinity2.RequiredComponents, x => x == typeof(TestComponentTwo));

            Assert.Single(groupWithAffinity3.RequiredComponents);
            Assert.Contains(groupWithAffinity3.RequiredComponents, x => x == typeof(TestComponentOne));

            Assert.Single(groupWithAffinity4.RequiredComponents);
            Assert.Contains(groupWithAffinity4.RequiredComponents, x => x == typeof(TestComponentOne));

            Assert.Single(groupWithAffinity5.RequiredComponents);
            Assert.Contains(groupWithAffinity5.RequiredComponents, x => x == typeof(TestComponentTwo));

            Assert.Single(groupWithAffinity6.RequiredComponents);
            Assert.Contains(groupWithAffinity6.RequiredComponents, x => x == typeof(TestComponentOne));
        }

        [Fact]
        public void should_throw_exception_when_no_group_system_interface_with_empty_group()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var reactToEntitySystemHandler = new GroupBindingSystemHandler(observableGroupManager);
            var dummySystem = new SystemMissingGroup();
            var propertyInfo = dummySystem.GetType().GetProperty(nameof(dummySystem.ObservableGroupA));
            
            try
            { reactToEntitySystemHandler.ProcessProperty(propertyInfo, dummySystem); }
            catch (MissingGroupSystemInterfaceException e)
            {
                Assert.Equal(e.Member, propertyInfo);
                Assert.Equal(e.System, dummySystem);
                return;
            }
            
            Assert.True(false, "The test should have thrown exception");
        }
        
        [Fact]
        public void should_populate_properties()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var reactToEntitySystemHandler = new GroupBindingSystemHandler(observableGroupManager);
            var dummySystem = new SystemWithAutoGroupPopulation();

            reactToEntitySystemHandler.SetupSystem(dummySystem);

            // This could be more specific but given other tests it seems fine for now
            observableGroupManager.Received(4).GetObservableGroup(Arg.Any<TestGroupA>(), Arg.Any<int[]>());
            observableGroupManager.Received(2).GetObservableGroup(Arg.Any<Group>(), Arg.Any<int[]>());
        }
    }
}