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
    public class DeletingOverlappingReactiveEntityTestSystem2 : IReactToEntitySystem
    {
        public IGroup Group => new Group()
            .WithComponent<ComponentWithReactiveProperty>()
            .WithComponent<TestComponentOne>();
        
        public Observable<Entity> ReactToEntity(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        { return entityComponentAccessor.GetComponent<ComponentWithReactiveProperty>(entity).SomeNumber.Select(x => entity); }

        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        { throw new Exception("Should Not Get Called"); }
    }
}