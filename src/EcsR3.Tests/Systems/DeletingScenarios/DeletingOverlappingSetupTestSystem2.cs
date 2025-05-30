using System;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Tests.Models;

namespace EcsR3.Tests.Systems.DeletingScenarios
{
    public class DeletingOverlappingSetupTestSystem2 : ISetupSystem
    {
        public IGroup Group => new Group()
            .WithComponent<ComponentWithReactiveProperty>()
            .WithComponent<TestComponentTwo>();

        public void Setup(IEntityComponentAccessor entityComponentAccessor, int entityId)
        { throw new Exception("Should Not Get Called"); }
    }
}