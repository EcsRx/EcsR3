using System.Collections.Generic;
using System.Linq;
using EcsR3.Components;
using EcsR3.Tests.Models;
using SystemsR3.Pools.Config;
using Xunit;

namespace EcsR3.Tests.EcsR3.Pools
{
    public class ComponentPoolTests
    {
        [Fact]
        public void should_allocate_up_front_components()
        {
            var initialSize = 100;

            var poolConfig = new PoolConfig(initialSize);
            var componentPool = new ComponentPool<TestComponentOne>(poolConfig);
            Assert.Equal(componentPool.Count, initialSize);
            Assert.Equal(componentPool.InternalComponents.Length, initialSize);
        }
        
        [Fact]
        public void should_correctly_identify_struct_types()
        {
            var classBasedComponentPool = new ComponentPool<TestComponentOne>();
            var structBasedComponentPool = new ComponentPool<TestStructComponentOne>();
            Assert.False(classBasedComponentPool.IsStructType);
            Assert.True(structBasedComponentPool.IsStructType);
        }
        
        [Fact]
        public void should_expand_explicitly_when_needed()
        {
            var expansionIterations = 20;
            var expansionSize = 10;
            var initialSize = 10;
            
            var poolConfig = new PoolConfig(initialSize);
            var componentPool = new ComponentPool<TestComponentOne>(poolConfig);
            var newSize = initialSize;
            for (var i = 0; i < expansionIterations; i++)
            {
                componentPool.Expand(expansionSize);
                newSize += expansionSize;

                Assert.Equal(componentPool.Count, newSize);
                Assert.Equal(componentPool.InternalComponents.Length, newSize);
            }            
        }
        
        [Fact]
        public void should_expand_automatically_when_needed()
        {
            var expansionIterations = 20;
            var expansionSize = 10;
            var initialSize = 10;
            
            var poolConfig = new PoolConfig(initialSize, expansionSize);
            var componentPool = new ComponentPool<TestComponentOne>(poolConfig);
            var newSize = initialSize;
            for (var i = 0; i < expansionIterations; i++)
            {
                componentPool.Expand();
                newSize += expansionSize;

                Assert.Equal(componentPool.Count, newSize);
                Assert.Equal(componentPool.InternalComponents.Length, newSize);
            }            
        }

        [Fact]
        public void should_allocate_correctly()
        {
            var initialSize = 1;
            
            var poolConfig = new PoolConfig(initialSize);
            var componentPool = new ComponentPool<TestComponentOne>(poolConfig);
            var allocation = componentPool.Allocate();
            
            Assert.Equal(0, allocation);
            Assert.Equal(0, componentPool.IndexesRemaining);
        }
        
        [Fact]
        public void should_expand_for_allocations_exceeding_count_correctly()
        {
            var expectedAllocationCount = 10;
            var expansionSize = 2;
            var initialSize = 2;
            var expectedAllocations = Enumerable.Range(0, expectedAllocationCount).ToList();
            var actualAllocations = new List<int>();
            
            var poolConfig = new PoolConfig(initialSize, expansionSize);
            var componentPool = new ComponentPool<TestComponentOne>(poolConfig);
            for (var i = 0; i < expectedAllocationCount; i++)
            {
                var allocation = componentPool.Allocate();
                actualAllocations.Add(allocation);
            }            
            
            Assert.Equal(expectedAllocationCount, actualAllocations.Count);
            Assert.All(actualAllocations, x => expectedAllocations.Contains(x));
            Assert.Equal(expectedAllocationCount, componentPool.InternalComponents.Length);
            Assert.Equal(expectedAllocationCount, componentPool.Count);
        }
        
        [Fact]
        public void should_request_index_pool_release_when_releasing_component()
        {
            var poolConfig = new PoolConfig(10);
            var componentPool = new ComponentPool<TestStructComponentOne>(poolConfig);
            var indexToUse = componentPool.IndexPool.AllocateInstance();
            var availableIndexesPrior = componentPool.IndexPool.AvailableIndexes.ToArray();
            componentPool.Release(indexToUse);
            var availableIndexesAfter = componentPool.IndexPool.AvailableIndexes.ToArray();
            
            Assert.NotSame(availableIndexesPrior, availableIndexesAfter);
            Assert.DoesNotContain(indexToUse, availableIndexesPrior);
            Assert.Contains(indexToUse, availableIndexesAfter);
        }
        
        [Fact]
        public void should_nullify_class_based_components_on_release()
        {
            var poolConfig = new PoolConfig(10);
            var componentPool = new ComponentPool<TestComponentOne>(poolConfig);
            var indexToUse = componentPool.IndexPool.AllocateInstance();
            componentPool.InternalComponents[indexToUse] = new TestComponentOne();
            
            componentPool.Release(indexToUse);
            
            Assert.True(componentPool.InternalComponents.All(x => x is null));
        }  
        
        [Fact]
        public void should_dispose_disposable_component_on_release()
        {
            var poolConfig = new PoolConfig(10);
            var componentPool = new ComponentPool<TestDisposableComponent>(poolConfig);
            var indexToUse = componentPool.IndexPool.AllocateInstance();
            var componentToUse = new TestDisposableComponent();
            componentPool.InternalComponents[indexToUse] = componentToUse;
            
            componentPool.Release(indexToUse);
            
            Assert.True(componentToUse.isDisposed);
        }

        [Fact]
        public void should_clear_and_be_reusable_afterwards()
        {
            var poolConfig = new PoolConfig(10);
            var componentPool = new ComponentPool<TestDisposableComponent>(poolConfig);
            var firstIndex = componentPool.IndexPool.AllocateInstance();
            var disposableComponent = new TestDisposableComponent();
            componentPool.Set(firstIndex, disposableComponent);
            componentPool.Clear();
            
            Assert.True(disposableComponent.isDisposed);
            Assert.Equal(poolConfig.InitialSize, componentPool.Count);
            Assert.Equal(poolConfig.InitialSize, componentPool.InternalComponents.Length);
            
            var secondIndex = componentPool.IndexPool.AllocateInstance();
            var secondComponent = new TestDisposableComponent();
            componentPool.Set(secondIndex, secondComponent);
            
            Assert.Equal(firstIndex, secondIndex);
            Assert.Equal(10, componentPool.Count);
            Assert.Contains(secondComponent, componentPool.InternalComponents);
        }
    }
}