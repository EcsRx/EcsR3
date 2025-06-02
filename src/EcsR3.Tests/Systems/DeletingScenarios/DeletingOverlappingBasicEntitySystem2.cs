using System;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Tests.Models;
using SystemsR3.Scheduling;

namespace EcsR3.Tests.Systems.DeletingScenarios
{
    public class DeletingOverlappingBasicEntitySystem2 : IBasicEntitySystem
    {
        public IGroup Group => new Group()
            .WithComponent<ComponentWithReactiveProperty>()
            .WithComponent<TestComponentThree>();

        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity, ElapsedTime elapsedTime)
        { throw new Exception("Should Not Be Called"); }
    }
}