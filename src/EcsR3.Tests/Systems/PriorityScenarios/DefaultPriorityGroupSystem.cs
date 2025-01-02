using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Systems;
using EcsR3.Systems;

namespace EcsR3.Tests.Systems.PriorityScenarios
{
    public class DefaultPriorityGroupSystem : IGroupSystem
    {
        public IGroup Group => null;
    }
    
    public class DefaultPrioritySetupSystem : ISetupSystem
    {
        public IGroup Group => null;
        public void Setup(IEntity entity){}
    }
    
    public class DefaultPriorityViewResolverSystem : IViewResolverSystem
    {
        public IGroup Group => null;
        public void Teardown(IEntity entity){}
        public void Setup(IEntity entity){}
    }
}