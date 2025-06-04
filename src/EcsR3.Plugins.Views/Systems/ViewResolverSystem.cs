using SystemsR3.Events;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Components;
using EcsR3.Plugins.Views.ViewHandlers;
using SystemsR3.Systems.Conventional;

namespace EcsR3.Plugins.Views.Systems
{
    public abstract class ViewResolverSystem : IViewResolverSystem, IManualSystem
    {
        public IEventSystem EventSystem { get; }

        public IViewHandler ViewHandler { get; private set; }
        public virtual IGroup Group => new Group(typeof(ViewComponent));
        
        protected ViewResolverSystem(IEventSystem eventSystem)
        { EventSystem = eventSystem; }

        public abstract IViewHandler CreateViewHandler();
        
        protected virtual void OnViewRemoved(IEntityComponentAccessor entityComponentAccessor, Entity entity, ViewComponent viewComponent)
        { ViewHandler.DestroyView(viewComponent.View); }

        protected abstract void OnViewCreated(IEntityComponentAccessor entityComponentAccessor, Entity entity, ViewComponent viewComponent);
        
        public virtual void Teardown(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var viewComponent = entityComponentAccessor.GetComponent<ViewComponent>(entity);
            OnViewRemoved(entityComponentAccessor, entity, viewComponent);
        }

        public virtual void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var viewComponent = entityComponentAccessor.GetComponent<ViewComponent>(entity);
            if (viewComponent.View != null) { return; }

            viewComponent.View = ViewHandler.CreateView();
            OnViewCreated(entityComponentAccessor, entity, viewComponent);
        }

        public void StartSystem()
        { ViewHandler = CreateViewHandler(); }

        public void StopSystem()
        {}
    }
}