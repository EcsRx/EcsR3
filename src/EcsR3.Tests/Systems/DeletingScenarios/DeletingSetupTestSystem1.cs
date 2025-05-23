using EcsR3.Collections.Entity;
using EcsR3.Entities;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Tests.Models;

namespace EcsR3.Tests.Systems.DeletingScenarios
{
    public class DeletingSetupTestSystem1 : ISetupSystem
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();

        public IEntityCollection EntityCollection { get; }

        public DeletingSetupTestSystem1(IEntityCollection entityCollection)
        { EntityCollection = entityCollection; }

        public void Setup(IEntity entity)
        { EntityCollection.Remove(entity.Id); }
    }
}