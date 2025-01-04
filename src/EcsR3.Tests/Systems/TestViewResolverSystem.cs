using System;
using SystemsR3.Events;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Components;
using EcsR3.Plugins.Views.Systems;
using EcsR3.Plugins.Views.ViewHandlers;

namespace EcsR3.Tests.Systems
{
    public class TestViewResolverSystem : ViewResolverSystem
    {
        public override IViewHandler ViewHandler { get; }
        public override IGroup Group { get; }

        public Action<IEntity> OnSetup { get; set; }
        public Action<IEntity> OnTeardown { get; set; }
        
        public TestViewResolverSystem(IEventSystem eventSystem, IGroup group) : base(eventSystem)
        {
            Group = group;
        }
        
        protected override void OnViewCreated(IEntity entity, ViewComponent viewComponent)
        {
            
        }

        public override void Setup(IEntity entity)
        {
            OnSetup?.Invoke(entity);
        }
        
        public override void Teardown(IEntity entity)
        {
            OnTeardown?.Invoke(entity);
        }
    }
}