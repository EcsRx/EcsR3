using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Systems;
using EcsR3.Systems;
using EcsR3.Systems.Reactive;

namespace EcsR3.Tests.Systems.PriorityScenarios
{
    public class DefaultPriorityGroupSystem : IGroupSystem
    {
        public IGroup Group => null;
    }
    
    public class DefaultPrioritySetupSystem : ISetupSystem
    {
        public IGroup Group => null;
        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity){}
    }
    
    public class DefaultPriorityViewResolverSystem : IViewResolverSystem
    {
        public IGroup Group => null;
        public void Teardown(IEntityComponentAccessor entityComponentAccessor, Entity entity){}
        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity){}
    }
}