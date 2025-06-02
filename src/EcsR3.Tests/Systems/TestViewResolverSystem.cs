using System;
using SystemsR3.Events;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Components;
using EcsR3.Plugins.Views.Systems;
using EcsR3.Plugins.Views.ViewHandlers;

namespace EcsR3.Tests.Systems
{
    public class TestViewResolverSystem : ViewResolverSystem
    {
        public override IGroup Group { get; }

        public Action<Entity> OnSetup { get; set; }
        public Action<Entity> OnTeardown { get; set; }
        
        public TestViewResolverSystem(IEventSystem eventSystem, IGroup group) : base(eventSystem)
        {
            Group = group;
        }

        public override IViewHandler CreateViewHandler() { return null; }

        protected override void OnViewCreated(IEntityComponentAccessor entityComponentAccessor, Entity entity, ViewComponent viewComponent)
        { }

        public override void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            OnSetup?.Invoke(entity);
        }
        
        public override void Teardown(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            OnTeardown?.Invoke(entity);
        }
    }
}