using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Systems;
using EcsR3.Systems;
using EcsR3.Systems.Reactive;
using SystemsR3.Attributes;

namespace EcsR3.Tests.Systems.PriorityScenarios
{
    [Priority(100)]
    public class HighestPriorityGroupSystem : IGroupSystem
    {
        public IGroup Group => null;
    }
    
    [Priority(101)]
    public class HighestPrioritySetupSystem : ISetupSystem
    {
        public IGroup Group => null;
        
        public void Setup(IEntityComponentAccessor entityComponentAccessor, int entityId) {}
    }
    
    [Priority(102)]
    public class HighestPriorityViewResolverSystem : IViewResolverSystem
    {
        public IGroup Group => null;
        public void Teardown(IEntityComponentAccessor entityComponentAccessor, int entityId){}
        public void Setup(IEntityComponentAccessor entityComponentAccessor, int entityId){}
    }
}