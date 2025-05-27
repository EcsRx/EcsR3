using System.Collections.Generic;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;
using SystemsR3.Tests.TestCode;
using Xunit;

namespace SystemsR3.Tests.SystemsR3.Pools;

public class ObjectPoolTests
{
    [Fact]
    public void should_not_populate_pool_on_creation()
    {
        var expectedSize = 5;
        var poolConfig = new PoolConfig(expectedSize, expectedSize);
        var objectPool = new TestObjectPool(poolConfig);
        
        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, Assert.Null);
    }
    
    [Fact]
    public void should_populate_existing_size_when_pre_allocating_without_size()
    {
        var expectedSize = 5;
        var poolConfig = new PoolConfig(expectedSize, expectedSize);
        var objectPool = new TestObjectPool(poolConfig);
        objectPool.PreAllocate();
        
        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
    }
    
    [Theory]
    [InlineData(10, 10)]
    [InlineData(100, 100)]
    [InlineData(0, 5)]
    public void should_populate_requested_size_when_pre_allocating_with_size(int preAllocationSize, int expectedSize)
    {
        var startingSize = 5;
        var poolConfig = new PoolConfig(startingSize, startingSize);
        var objectPool = new TestObjectPool(poolConfig);
        objectPool.PreAllocate(preAllocationSize);
        
        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
    }
    
    [Theory]
    [InlineData(10, 5, 5)]
    [InlineData(10, 10, 100)]
    [InlineData(100, 10, 10)]
    [InlineData(0, 5, 10)]
    public void should_not_exceed_max_size_on_pre_allocations(int preAllocationSize, int expectedSize, int maxSize)
    {
        var startingSize = 5;
        var poolConfig = new PoolConfig(startingSize, startingSize, maxSize);
        var objectPool = new TestObjectPool(poolConfig);
        objectPool.PreAllocate(preAllocationSize);
        
        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
    }
    
    [Fact]
    public void should_populate_existing_size_when_allocating_if_needed()
    {
        var expectedSize = 5;
        var poolConfig = new PoolConfig(expectedSize, expectedSize);
        var objectPool = new TestObjectPool(poolConfig);
        objectPool.AllocateInstance();
        
        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
    }
    
    [Theory]
    [InlineData(5, 5, 5)]
    [InlineData(5, 10, 6)]
    [InlineData(5, 10, 10)]
    [InlineData(2, 12, 11)]
    public void should_expand_on_expansion_size_and_populate_when_allocating_over_existing_size(int expansionSize, int expectedSize, int numberToAllocate)
    {
        var poolConfig = new PoolConfig(expansionSize, expansionSize);
        var objectPool = new TestObjectPool(poolConfig);
        for (var i = 0; i < numberToAllocate; i++)
        { objectPool.AllocateInstance(); }

        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
    }
    
    [Fact]
    public void should_not_expand_if_deallocated_objects_exist_on_allocation()
    {
        var expectedSize = 2;
        var poolConfig = new PoolConfig(expectedSize, expectedSize);
        var objectPool = new TestObjectPool(poolConfig);

        for (var i = 0; i < 10; i++)
        {
            var instance = objectPool.AllocateInstance();
            objectPool.ReleaseInstance(instance);
        }

        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
    }
    
    [Fact]
    public void should_ignore_release_of_already_released_object()
    {
        var expectedSize = 2;
        var poolConfig = new PoolConfig(expectedSize, expectedSize);
        var objectPool = new TestObjectPool(poolConfig);

        var instance = objectPool.AllocateInstance();
        objectPool.ReleaseInstance(instance);
        objectPool.ReleaseInstance(instance);
        objectPool.ReleaseInstance(instance);
        objectPool.ReleaseInstance(instance);

        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
    }
    
    [Fact]
    public void should_clear_all_data_and_destroy_all_instances_on_clearing()
    {
        var expectedSize = 2;
        var poolConfig = new PoolConfig(expectedSize, expectedSize);
        var objectPool = new TestObjectPool(poolConfig);

        var instance1 = objectPool.AllocateInstance();
        var instance2 = objectPool.AllocateInstance();
        
        objectPool.Clear();

        Assert.Empty(objectPool.Objects);
        Assert.Empty(objectPool.IndexPool.AvailableIndexes);
        Assert.True(instance1.IsDestroyed);
        Assert.True(instance2.IsDestroyed);
    }
    
    [Fact]
    public void should_allow_reuse_after_clearing_creating_new_instances()
    {
        var expansionSize = 2;
        var expectedSize = expansionSize * 2;
        var poolConfig = new PoolConfig(expansionSize, expansionSize);
        var objectPool = new TestObjectPool(poolConfig);

        var instance1 = objectPool.AllocateInstance();
        var instance2 = objectPool.AllocateInstance();
        
        objectPool.Clear();
        
        var instance4 = objectPool.AllocateInstance();
        var instance5 = objectPool.AllocateInstance();
        var instance6 = objectPool.AllocateInstance();

        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
        Assert.All(objectPool, x => Assert.False(x.IsDestroyed));
        Assert.True(instance1.IsDestroyed);
        Assert.True(instance2.IsDestroyed);
    }

    [Fact]
    public void should_allow_multiple_expansions_and_allocations_then_release_successfully()
    {
        var amountToFirstAllocate = 30;
        var amountToSecondAllocate = 40;
        var poolConfig = new PoolConfig(5, 10);
        var objectPool = new TestObjectPool(poolConfig);
        var allocatedObjects = new List<TestPooledObject>();
        
        objectPool.Expand(15);
        
        for (var i = 0; i < amountToFirstAllocate; i++)
        { allocatedObjects.Add(objectPool.AllocateInstance()); }
        
        objectPool.Expand(15);

        for (var i = 0; i < amountToSecondAllocate; i++)
        { allocatedObjects.Add(objectPool.AllocateInstance()); }
        
        for(var i=0;i<allocatedObjects.Count;i++)
        { objectPool.ReleaseInstance(allocatedObjects[i]); }

        Assert.Equal(70, allocatedObjects.Count);
        Assert.Equal(75, objectPool.Objects.Length);
    }
}