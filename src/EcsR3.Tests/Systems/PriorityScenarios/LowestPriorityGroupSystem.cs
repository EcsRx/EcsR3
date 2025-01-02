﻿using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Systems;
using EcsR3.Systems;
using SystemsR3.Attributes;

namespace EcsR3.Tests.Systems.PriorityScenarios
{
    [Priority(-100)]
    public class LowestPriorityGroupSystem : IGroupSystem
    {
        public IGroup Group => null;
    }
    
    [Priority(-101)]
    public class LowestPrioritySetupSystem : ISetupSystem
    {
        public IGroup Group => null;
        
        public void Setup(IEntity entity) {}
    }
    
    [Priority(-102)]
    public class LowestPriorityViewResolverSystem : IViewResolverSystem
    {
        public IGroup Group => null;
        public void Teardown(IEntity entity){}
        public void Setup(IEntity entity){}
    }
}