using EcsR3.Plugins.Views.Pooling;
using EcsR3.Plugins.Views.ViewHandlers;
using NSubstitute;
using Xunit;

namespace EcsR3.Tests.Plugins.Views;

public class ViewPoolTests
{
    [Fact]
    public void should_not_allocate_views_on_creation()
    {
        var mockViewHandler = Substitute.For<IViewHandler>();
        var viewPool = new ViewPool(10, mockViewHandler);
        
        Assert.Empty(viewPool.PooledObjects);
    }
    
    [Fact]
    public void should_pre_allocate_only_requested_size_on_direct_call()
    {
        var mockViewHandler = Substitute.For<IViewHandler>();
        var viewPool = new ViewPool(10, mockViewHandler);
        viewPool.PreAllocate(5);
        
        Assert.Equal(5, viewPool.PooledObjects.Count);
    }
    
    [Fact]
    public void should_expand_if_allocating_with_not_enough_instances_from_empty()
    {
        var mockViewHandler = Substitute.For<IViewHandler>();
        var viewPool = new ViewPool(10, mockViewHandler);
        var a = viewPool.AllocateInstance();
        
        Assert.Equal(10, viewPool.PooledObjects.Count);
        mockViewHandler.Received(10).CreateView();
    }
    
    [Fact]
    public void should_expand_if_allocating_with_not_enough_instances_from_existing()
    {
        var mockViewHandler = Substitute.For<IViewHandler>();
        var viewPool = new ViewPool(10, mockViewHandler);

        for (var i = 0; i < 11; i++)
        {
            viewPool.AllocateInstance();
        }
        
        Assert.Equal(20, viewPool.PooledObjects.Count);
        mockViewHandler.Received(20).CreateView();
    }
}