using SystemsR3.Events;
using SystemsR3.Systems.Conventional;
using EcsR3.Entities;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Components;
using EcsR3.Plugins.Views.Pooling;
using EcsR3.Systems;

namespace EcsR3.Plugins.Views.Systems
{
    public abstract class PooledViewResolverSystem : IViewResolverSystem, IManualSystem
    {
        public IEventSystem EventSystem { get; }

        public virtual IGroup Group => new Group(typeof(ViewComponent));
        public IViewPool ViewPool { get; private set; }

        protected PooledViewResolverSystem(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;
        }

        protected abstract IViewPool CreateViewPool();
        protected abstract void OnPoolStarting();
        protected abstract void OnViewRecycled(object view, IEntity entity);
        protected abstract void OnViewAllocated(object view, IEntity entity);

        protected virtual void RecycleView(IEntity entity, ViewComponent viewComponent)
        {
            var view = viewComponent.View;
            ViewPool.ReleaseInstance(view);
            viewComponent.View = null;
            OnViewRecycled(view, entity);
        }

        protected virtual object AllocateView(IEntity entity, ViewComponent viewComponent)
        {
            var viewToAllocate = ViewPool.AllocateInstance();
            viewComponent.View = viewToAllocate;
            OnViewAllocated(viewToAllocate, entity);
            return viewToAllocate;
        }

        public void Setup(IEntity entity)
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            AllocateView(entity, viewComponent);
        }

        public virtual void Teardown(IEntity entity)
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            RecycleView(entity, viewComponent);
        }

        public void StartSystem()
        {
            ViewPool = CreateViewPool();
            OnPoolStarting();
        }

        public void StopSystem()
        { ViewPool.EmptyPool(); }
    }
}