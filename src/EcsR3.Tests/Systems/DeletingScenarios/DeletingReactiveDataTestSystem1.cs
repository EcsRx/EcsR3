using EcsR3.Collections.Entity;
using EcsR3.Entities;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Tests.Models;
using R3;

namespace EcsR3.Tests.Systems.DeletingScenarios
{
    public class DeletingReactiveDataTestSystem1 : IReactToDataSystem<int>
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();
        public IEntityCollection EntityCollection { get; }

        public DeletingReactiveDataTestSystem1(IEntityCollection entityCollection)
        { EntityCollection = entityCollection; }

        public Observable<int> ReactToData(IEntity entity)
        { return entity.GetComponent<ComponentWithReactiveProperty>().SomeNumber; }

        public void Process(IEntity entity, int reactionData)
        { EntityCollection.RemoveEntity(entity.Id); }
    }
}