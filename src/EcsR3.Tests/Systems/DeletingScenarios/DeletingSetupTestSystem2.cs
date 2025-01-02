using System;
using EcsR3.Entities;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Tests.Models;

namespace EcsR3.Tests.Systems.DeletingScenarios
{
    public class DeletingSetupTestSystem2 : ISetupSystem
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();

        public void Setup(IEntity entity)
        { throw new Exception("Should Not Get Called"); }
    }
}