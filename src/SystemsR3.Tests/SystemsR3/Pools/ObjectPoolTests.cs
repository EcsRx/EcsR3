using SystemsR3.Tests.TestCode;
using Xunit;

namespace SystemsR3.Tests.SystemsR3.Pools;

public class ObjectPoolTests
{
    [Fact]
    public void should_not_populate_pool_on_creation()
    {
        var expectedSize = 5;
        var objectPool = new TestObjectPool(expectedSize);
        
        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, Assert.Null);
    }
    
    [Fact]
    public void should_populate_existing_size_when_pre_allocating_without_size()
    {
        var expectedSize = 5;
        var objectPool = new TestObjectPool(expectedSize);
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
        var objectPool = new TestObjectPool(startingSize);
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
        var objectPool = new TestObjectPool(startingSize) { MaxSize = maxSize };
        objectPool.PreAllocate(preAllocationSize);
        
        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
    }
    
    [Fact]
    public void should_populate_existing_size_when_allocating_if_needed()
    {
        var expectedSize = 5;
        var objectPool = new TestObjectPool(expectedSize);
        objectPool.Allocate();
        
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
        var objectPool = new TestObjectPool(expansionSize);
        for (var i = 0; i < numberToAllocate; i++)
        { objectPool.Allocate(); }

        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
    }
    
    [Fact]
    public void should_not_expand_if_deallocated_objects_exist_on_allocation()
    {
        var expectedSize = 2;
        var objectPool = new TestObjectPool(expectedSize);

        for (var i = 0; i < 10; i++)
        {
            var instance = objectPool.Allocate();
            objectPool.Release(instance);
        }

        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
    }
    
    [Fact]
    public void should_ignore_release_of_already_released_object()
    {
        var expectedSize = 2;
        var objectPool = new TestObjectPool(expectedSize);

        var instance = objectPool.Allocate();
        objectPool.Release(instance);
        objectPool.Release(instance);
        objectPool.Release(instance);
        objectPool.Release(instance);

        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
    }
    
    [Fact]
    public void should_clear_all_data_and_destroy_all_instances_on_clearing()
    {
        var expectedSize = 2;
        var objectPool = new TestObjectPool(expectedSize);

        var instance1 = objectPool.Allocate();
        var instance2 = objectPool.Allocate();
        
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
        var objectPool = new TestObjectPool(expansionSize);

        var instance1 = objectPool.Allocate();
        var instance2 = objectPool.Allocate();
        
        objectPool.Clear();
        
        var instance4 = objectPool.Allocate();
        var instance5 = objectPool.Allocate();
        var instance6 = objectPool.Allocate();

        Assert.Equal(expectedSize, objectPool.Objects.Length);
        Assert.All(objectPool, x => Assert.IsType<TestPooledObject>(x));
        Assert.All(objectPool, x => Assert.False(x.IsDestroyed));
        Assert.True(instance1.IsDestroyed);
        Assert.True(instance2.IsDestroyed);
    }
}