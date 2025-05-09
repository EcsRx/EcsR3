using EcsR3.Plugins.Views.Pooling;
using EcsR3.Plugins.Views.ViewHandlers;
using NSubstitute;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;
using Xunit;

namespace EcsR3.Tests.Plugins.Views;

public class ViewPoolTests
{
    [Fact]
    public void should_not_allocate_views_on_creation()
    {
        var mockViewHandler = Substitute.For<IViewHandler>();

        var poolConfig = new PoolConfig(10, 10);
        var viewPool = new ViewPool(mockViewHandler, poolConfig);
        
        Assert.Empty(viewPool.PooledObjects);
    }
    
    [Fact]
    public void should_pre_allocate_only_requested_size_on_direct_call()
    {
        var mockViewHandler = Substitute.For<IViewHandler>();
        var poolConfig = new PoolConfig(10, 10);
        var viewPool = new ViewPool(mockViewHandler, poolConfig);
        viewPool.PreAllocate(5);
        
        Assert.Equal(5, viewPool.PooledObjects.Count);
    }
    
    [Fact]
    public void should_expand_if_allocating_with_not_enough_instances_from_empty()
    {
        var mockViewHandler = Substitute.For<IViewHandler>();
        var poolConfig = new PoolConfig(10, 10);
        var viewPool = new ViewPool(mockViewHandler, poolConfig);
        var a = viewPool.AllocateInstance();
        
        Assert.Equal(10, viewPool.PooledObjects.Count);
        mockViewHandler.Received(10).CreateView();
    }
    
    [Fact]
    public void should_expand_if_allocating_with_not_enough_instances_from_existing()
    {
        var mockViewHandler = Substitute.For<IViewHandler>();
        var poolConfig = new PoolConfig(10, 10);
        var viewPool = new ViewPool(mockViewHandler, poolConfig);

        for (var i = 0; i < 11; i++)
        {
            viewPool.AllocateInstance();
        }
        
        Assert.Equal(20, viewPool.PooledObjects.Count);
        mockViewHandler.Received(20).CreateView();
    }
}