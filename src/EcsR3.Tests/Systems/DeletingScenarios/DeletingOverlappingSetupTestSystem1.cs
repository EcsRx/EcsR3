using EcsR3.Collections.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Reactive;
using EcsR3.Tests.Models;

namespace EcsR3.Tests.Systems.DeletingScenarios
{
    public class DeletingOverlappingSetupTestSystem1 : ISetupSystem
    {
        public IGroup Group => new Group()
            .WithComponent<ComponentWithReactiveProperty>()
            .WithComponent<TestComponentTwo>();

        public IEntityCollection EntityCollection { get; }

        public DeletingOverlappingSetupTestSystem1(IEntityCollection entityCollection)
        { EntityCollection = entityCollection; }

        public void Setup(IEntityComponentAccessor entityComponentAccessor, int entityId)
        { EntityCollection.Remove(entityId); }
    }
}