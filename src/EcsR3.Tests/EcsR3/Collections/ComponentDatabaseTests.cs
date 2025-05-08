using System;
using System.Collections.Generic;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Tests.Models;
using NSubstitute;
using SystemsR3.Pools;
using Xunit;

namespace EcsR3.Tests.EcsRx.Collections
{
    public class ComponentDatabaseTests
    {
        [Fact]
        public void should_initialize_pools_for_given_component_types()
        {
            var expectedSize = 10;
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentTypeMappings().Returns(fakeComponentTypes);
            
            var databaseConfig = new ComponentDatabaseConfig(expectedSize, expectedSize);
            var database = new ComponentDatabase(mockComponentLookup, databaseConfig);           
            Assert.Equal(fakeComponentTypes.Count, database.ComponentData.Length);
            Assert.Equal(expectedSize, database.ComponentData[0].Count);
            Assert.Equal(expectedSize, database.ComponentData[1].Count);
            Assert.Equal(expectedSize, database.ComponentData[2].Count);
        }
        
        [Fact]
        public void should_initialize_pools_for_given_component_types_but_not_pre_allocate_them_if_set_to()
        {
            var expectedSize = 0;
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentTypeMappings().Returns(fakeComponentTypes);

            var databaseConfig = new ComponentDatabaseConfig(expectedSize, expectedSize) { OnlyPreAllocatePoolsWithConfig = true };
            var database = new ComponentDatabase(mockComponentLookup, databaseConfig);           
            Assert.Equal(fakeComponentTypes.Count, database.ComponentData.Length);
            Assert.Equal(expectedSize, database.ComponentData[0].Count);
            Assert.Equal(expectedSize, database.ComponentData[1].Count);
            Assert.Equal(expectedSize, database.ComponentData[2].Count);
        }
        
        [Fact]
        public void should_initialize_pools_for_given_component_types_with_custom_config_if_provided()
        {
            var expectedComponentOneSize = 3;
            var expectedComponentTwoSize = 6;
            var expectedDefaultSize = 9;
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentTypeMappings().Returns(fakeComponentTypes);

            var databaseConfig = new ComponentDatabaseConfig(expectedDefaultSize)
            {
                PoolSpecificConfig =
                {
                    { typeof(TestComponentOne), new PoolConfig(expectedComponentOneSize) },
                    { typeof(TestComponentTwo), new PoolConfig(expectedComponentTwoSize) }
                }
            };
            var database = new ComponentDatabase(mockComponentLookup, databaseConfig);           
            Assert.Equal(fakeComponentTypes.Count, database.ComponentData.Length);
            Assert.Equal(expectedComponentOneSize, database.ComponentData[0].Count);
            Assert.Equal(expectedComponentTwoSize, database.ComponentData[1].Count);
            Assert.Equal(expectedDefaultSize, database.ComponentData[2].Count);
        }
        
        [Fact]
        public void should_only_pre_allocate_pools_for_given_component_types_with_custom_config_if_provided()
        {
            var expectedComponentOneSize = 3;
            var expectedComponentTwoSize = 6;
            var expectedComponentThreeSize = 0;
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentTypeMappings().Returns(fakeComponentTypes);

            var databaseConfig = new ComponentDatabaseConfig(10)
            {
                OnlyPreAllocatePoolsWithConfig = true,
                PoolSpecificConfig =
                {
                    { typeof(TestComponentOne), new PoolConfig(expectedComponentOneSize) },
                    { typeof(TestComponentTwo), new PoolConfig(expectedComponentTwoSize) }
                }
            };
            var database = new ComponentDatabase(mockComponentLookup, databaseConfig);           
            Assert.Equal(fakeComponentTypes.Count, database.ComponentData.Length);
            Assert.Equal(expectedComponentOneSize, database.ComponentData[0].Count);
            Assert.Equal(expectedComponentTwoSize, database.ComponentData[1].Count);
            Assert.Equal(expectedComponentThreeSize, database.ComponentData[2].Count);
        }
        
        [Fact]
        public void should_correctly_allocate_instance_when_adding()
        {
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentTypeMappings().Returns(new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0}
            });
            
            var database = new ComponentDatabase(mockComponentLookup);
            var allocation = database.Allocate(0);
            
            Assert.Equal(0, allocation);
        }
        
        [Fact]
        public void should_correctly_remove_instance()
        {
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentTypeMappings().Returns(new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0}
            });

            var mockExpandingArray = Substitute.For<IComponentPool>();
            
            var database = new ComponentDatabase(mockComponentLookup);
            database.ComponentData.SetValue(mockExpandingArray, 0);
            database.Remove(0, 0);
            
            mockExpandingArray.Received(1).Release(0);
        }
        
        [Fact]
        public void should_dispose_component_when_removed()
        {
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentTypeMappings().Returns(new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0}
            });

            var mockExpandingArray = Substitute.For<IComponentPool>();
            
            var database = new ComponentDatabase(mockComponentLookup);
            database.ComponentData.SetValue(mockExpandingArray, 0);
            database.Remove(0, 0);
            
            mockExpandingArray.Received(1).Release(0);
        }
    }
}