using EcsR3.Plugins.Views.ViewHandlers;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace EcsR3.Plugins.Views.Pools
{
    public class ViewPool : GenericPool<object>
    {
        public IViewHandler ViewHandler { get; }

        public ViewPool(IViewHandler viewHandler, PoolConfig poolConfig = null) : base(poolConfig)
        {
            ViewHandler = viewHandler;
        }

        public override object Create()
        {
            var instance = ViewHandler.CreateView();
            ViewHandler.SetActiveState(instance, false);
            return instance;
        }

        public override void Destroy(object instance)
        { ViewHandler.DestroyView(instance); }

        public override void OnAllocated(object instance)
        {
            base.OnAllocated(instance);
            ViewHandler.SetActiveState(instance, true);
        }

        public override void OnReleased(object instance)
        {
            base.OnReleased(instance);
            ViewHandler.SetActiveState(instance, false);
        }
    }
}