using System;
using EcsR3.Entities;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Tests.Models;
using R3;

namespace EcsR3.Tests.Systems.DeletingScenarios
{
    public class DeletingReactiveEntityTestSystem2 : IReactToEntitySystem
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();
        
        public Observable<IEntity> ReactToEntity(IEntity entity)
        { return entity.GetComponent<ComponentWithReactiveProperty>().SomeNumber.Select(x => entity); }

        public void Process(IEntity entity)
        { throw new Exception("Should Not Get Called"); }
    }
}