using System;
using System.Linq;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Entities.Routing;
using EcsR3.Extensions;
using EcsR3.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsR3.Tests.EcsR3
{
    public class EntityTests
    {
        [Fact]
        public void should_raise_event_when_adding_components()
        {
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            componentTypeLookup.AllComponentTypeIds.Returns(new []{0,1,2,3});
            componentTypeLookup.GetComponentTypeId(Arg.Any<Type>()).Returns(2);
            
            var entity = new Entity(1, componentDatabase, componentTypeLookup, entityChangeRouter);
            var dummyComponent = Substitute.For<IComponent>();

            entity.AddComponents(dummyComponent);
            
            entityChangeRouter.Received(1).PublishEntityAddedComponents(1, Arg.Is<int[]>(x => x.Contains(2)));
        }
        
        /* NSubstitute doesnt support ref returns currently
        [Fact]
        public void should_raise_event_when_adding_explicit_component()
        {
            var componentDatabase = Substitute.For<IComponentDatabase>();
            componentDatabase.GetRef<TestStructComponentOne>(Arg.Any<int>(), Arg.Any<int>()).Returns(new TestStructComponentOne());
            
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            
            var entity = new Entity(1, componentDatabase, componentTypeLookup);

            var wasCalled = false;
            entity.ComponentsAdded.Subscribe(x => wasCalled = true);

            entity.AddComponent<TestStructComponentOne>(0);
            Assert.True(wasCalled);
        }*/

        [Fact]
        public void should_raise_event_when_removing_component_that_exists()
        {
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[]{0});
            
            var entity = new Entity(1, componentDatabase, componentTypeLookup, entityChangeRouter);
            var dummyComponent = Substitute.For<IComponent>();

            entity.InternalComponentAllocations[0] = 1;

            entity.RemoveComponents(dummyComponent.GetType());
            
            entityChangeRouter.Received(1).PublishEntityRemovingComponents(1, Arg.Is<int[]>(x => x.Contains(0)));
            entityChangeRouter.Received(1).PublishEntityRemovedComponents(1, Arg.Is<int[]>(x => x.Contains(0)));
        }
        
        [Fact]
        public void should_not_raise_events_or_throw_when_removing_non_existent_components()
        {
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] { 0 });
            
            var entity = new Entity(1, componentDatabase, componentTypeLookup, entityChangeRouter);
            
            entity.RemoveComponents(typeof(TestComponentOne));
            entityChangeRouter.DidNotReceive().PublishEntityRemovingComponents(1, Arg.Is<int[]>(x => x.Contains(0)));
            entityChangeRouter.DidNotReceive().PublishEntityRemovedComponents(1, Arg.Is<int[]>(x => x.Contains(0)));
        }

        [Fact]
        public void should_return_true_when_entity_has_all_components()
        {
            var fakeEntityId = 1;
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] {0, 1});
            componentTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(0);
            componentTypeLookup.GetComponentTypeId(typeof(TestComponentTwo)).Returns(1);
            
            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup, entityChangeRouter);
            entity.InternalComponentAllocations[0] = 1;
            entity.InternalComponentAllocations[1] = 1;
            Assert.True(entity.HasAllComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }

        [Fact]
        public void should_return_false_when_entity_does_not_match_all_components()
        {
            var fakeEntityId = 1;
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] {0, 1});
            componentTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(0);
            componentTypeLookup.GetComponentTypeId(typeof(TestComponentTwo)).Returns(1);
            
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup, entityChangeRouter);
            entity.InternalComponentAllocations[0] = 1;
            Assert.False(entity.HasAllComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }
        
        [Fact]
        public void should_return_true_when_entity_has_any_components()
        {
            var fakeEntityId = 1;
            
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] {0, 1});
            componentTypeLookup.GetComponentTypeId(typeof(TestComponentOne)).Returns(0);
            componentTypeLookup.GetComponentTypeId(typeof(TestComponentTwo)).Returns(1);
            
            var componentDatabase = Substitute.For<IComponentDatabase>();
            
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup, entityChangeRouter);
            entity.InternalComponentAllocations[0] = 1;
            
            Assert.True(entity.HasAnyComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }

        [Fact]
        public void should_return_false_when_entity_does_not_match_any_components()
        {
            var fakeEntityId = 1;
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            componentTypeLookup.AllComponentTypeIds.Returns(new int[1]);
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup, entityChangeRouter);
            
            Assert.False(entity.HasAnyComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }

        [Fact]
        public void should_add_components_in_parameter_order()
        {
            var fakeEntityId = 1;
            var fakeComponents = new IComponent[] {new TestComponentOne(), new TestComponentTwo(), new TestComponentThree()};

            var componentDatabase = Substitute.For<IComponentDatabase>();
            componentDatabase.Allocate(Arg.Any<int>()).Returns(1);
            
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] {0, 1, 2});
            componentTypeLookup.GetComponentTypeId(fakeComponents[0].GetType()).Returns(0);
            componentTypeLookup.GetComponentTypeId(fakeComponents[1].GetType()).Returns(1);
            componentTypeLookup.GetComponentTypeId(fakeComponents[2].GetType()).Returns(2);
            
            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
            
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup, entityChangeRouter);
            entity.AddComponents(fakeComponents);
            
            Received.InOrder(() => {
                componentDatabase.Allocate(0);
                componentDatabase.Set(0, 1, fakeComponents[0]);
                componentDatabase.Allocate(1);
                componentDatabase.Set(1, 1, fakeComponents[1]);
                componentDatabase.Allocate(2);
                componentDatabase.Set(2, 1, fakeComponents[2]);
            });
        }

        [Fact]
        public void should_return_all_components_allocated_to_the_entity()
        {
            var fakeEntityId = 1;
            var components = new IComponent[] {new TestComponentOne(), new TestComponentTwo(), new TestComponentThree()};
            var componentDatabase = Substitute.For<IComponentDatabase>();
            componentDatabase.Allocate(Arg.Any<int>()).Returns(0);
            componentDatabase.Get<IComponent>(Arg.Any<int>(), Arg.Any<int>()).Returns(info => components[info.ArgAt<int>(0)]);
            
            var entityChangeRouter = Substitute.For<IEntityChangeRouter>();
                
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] {0, 1, 2});
            componentTypeLookup.GetComponentTypeId(components[0].GetType()).Returns(0);
            componentTypeLookup.GetComponentTypeId(components[1].GetType()).Returns(1);
            componentTypeLookup.GetComponentTypeId(components[2].GetType()).Returns(2);
            
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup, entityChangeRouter);
            entity.AddComponents(components);
            Assert.Equal(components, entity.Components);
        }
    }
}
