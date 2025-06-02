using System;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Reactive;
using EcsR3.Tests.Models;
using R3;

namespace EcsR3.Tests.Systems.DeletingScenarios
{
    public class DeletingReactiveDataTestSystem2 : IReactToDataSystem<int>
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();

        public Observable<int> ReactToData(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        { return entityComponentAccessor.GetComponent<ComponentWithReactiveProperty>(entity).SomeNumber; }

        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity, int reactionData)
        { throw new Exception("Should Not Get Called"); }
    }
}