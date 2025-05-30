
using EcsR3.Plugins.Views.Pools;
using EcsR3.Plugins.Views.ViewHandlers;
using NSubstitute;
using SystemsR3.Pools.Config;
using Xunit;

namespace EcsR3.Tests.Plugins.Views;

public class ViewPoolTests
{
    [Fact]
    public void should_get_view_handler_to_create_instance_and_set_to_false_on_creation()
    {
        var mockViewHandler = Substitute.For<IViewHandler>();
        mockViewHandler.CreateView().Returns(new object());
        
        var poolConfig = new PoolConfig(2);
        var viewPool = new ViewPool(mockViewHandler, poolConfig);
        
        var instance = viewPool.Allocate();
        
        mockViewHandler.Received(2).CreateView();
        mockViewHandler.Received(poolConfig.InitialSize).SetActiveState(Arg.Any<object>(), false);
        Assert.NotNull(instance);
    }
    
    [Fact]
    public void should_get_view_handler_to_destroy_instances()
    {
        var mockViewHandler = Substitute.For<IViewHandler>();
        mockViewHandler.CreateView().Returns(new object());
        
        var poolConfig = new PoolConfig(2);
        var viewPool = new ViewPool(mockViewHandler, poolConfig);
        
        viewPool.PreAllocate();
        viewPool.Clear();
        
        mockViewHandler.Received(2).DestroyView(Arg.Any<object>());
    }
    
    [Fact]
    public void should_get_view_handler_to_set_active_states_on_allocation_and_release()
    {
        var mockViewHandler = Substitute.For<IViewHandler>();
        mockViewHandler.CreateView().Returns(new object());
        
        var poolConfig = new PoolConfig(2);
        var viewPool = new ViewPool(mockViewHandler, poolConfig);
        
        var instance = viewPool.Allocate();
        viewPool.Release(instance);
        
        mockViewHandler.Received(1).SetActiveState(Arg.Any<object>(), true);
        mockViewHandler.Received(poolConfig.InitialSize+1).SetActiveState(Arg.Any<object>(), false);
    }
}