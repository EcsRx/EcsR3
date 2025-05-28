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

        public Action<int> OnSetup { get; set; }
        public Action<int> OnTeardown { get; set; }
        
        public TestViewResolverSystem(IEventSystem eventSystem, IGroup group) : base(eventSystem)
        {
            Group = group;
        }

        public override IViewHandler CreateViewHandler() { return null; }

        protected override void OnViewCreated(IEntityComponentAccessor entityComponentAccessor, int entityId, ViewComponent viewComponent)
        { }

        public override void Setup(IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            OnSetup?.Invoke(entityId);
        }
        
        public override void Teardown(IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            OnTeardown?.Invoke(entityId);
        }
    }
}