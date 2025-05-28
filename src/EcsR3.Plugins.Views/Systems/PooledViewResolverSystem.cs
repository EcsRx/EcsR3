using SystemsR3.Events;
using SystemsR3.Systems.Conventional;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Components;
using EcsR3.Plugins.Views.Pools;
using EcsR3.Plugins.Views.ViewHandlers;
using SystemsR3.Pools.Config;

namespace EcsR3.Plugins.Views.Systems
{
    public abstract class PooledViewResolverSystem : IViewResolverSystem, IManualSystem
    {
        public IEventSystem EventSystem { get; }
        
        public virtual IGroup Group => new Group(typeof(ViewComponent));

        public virtual PoolConfig PoolConfig => new PoolConfig(); 
        public ViewPool ViewPool { get; private set; }
        public IViewHandler ViewHandler { get; private set; }

        protected PooledViewResolverSystem(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;
        }

        protected virtual ViewPool CreateViewPool() { return new ViewPool(ViewHandler, PoolConfig);}
        public abstract IViewHandler CreateViewHandler();
        
        protected virtual void OnPoolStarting(){}
        protected abstract void OnViewRecycled(IEntityComponentAccessor entityComponentAccessor, object view, int entity);
        protected abstract void OnViewAllocated(IEntityComponentAccessor entityComponentAccessor, object view, int entity);

        protected virtual void RecycleView(IEntityComponentAccessor entityComponentAccessor, int entityId, ViewComponent viewComponent)
        {
            var view = viewComponent.View;
            ViewPool.ReleaseInstance(view);
            viewComponent.View = null;
            OnViewRecycled(entityComponentAccessor, view, entityId);
        }

        protected virtual object AllocateView(IEntityComponentAccessor entityComponentAccessor, int entityId, ViewComponent viewComponent)
        {
            var viewToAllocate = ViewPool.AllocateInstance();
            viewComponent.View = viewToAllocate;
            OnViewAllocated(entityComponentAccessor, viewToAllocate, entityId);
            return viewToAllocate;
        }
        
        public void Teardown(IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            var viewComponent = entityComponentAccessor.GetComponent<ViewComponent>(entityId);
            RecycleView(entityComponentAccessor, entityId, viewComponent);
        }

        public void Setup(IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            var viewComponent = entityComponentAccessor.GetComponent<ViewComponent>(entityId);
            AllocateView(entityComponentAccessor, entityId, viewComponent);
        }

        public void StartSystem()
        {
            ViewHandler = CreateViewHandler();
            ViewPool = CreateViewPool();
            OnPoolStarting();
        }

        public void StopSystem()
        { ViewPool.Clear(); }
    }
}