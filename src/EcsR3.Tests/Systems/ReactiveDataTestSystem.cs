using System;
using EcsR3.Entities;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Tests.Models;
using R3;

namespace EcsR3.Tests.Systems
{
    public class ReactiveDataTestSystem : IReactToDataSystem<float>
    {
        public IGroup Group => new Group().WithComponent<TestComponentOne>();

        public Observable<float> ReactToData(IEntity entity)
        {
            return Observable.Timer(TimeSpan.FromSeconds(1)).Select(x => 0.1f);
        }

        public void Process(IEntity entity, float reactionData)
        {
            throw new NotImplementedException();
        }
    }
}