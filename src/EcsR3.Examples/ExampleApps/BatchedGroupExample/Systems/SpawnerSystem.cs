using System;
using SystemsR3.Systems.Conventional;
using EcsR3.Blueprints;
using EcsR3.Collections.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Blueprints;
using EcsR3.Extensions;
using R3;

namespace EcsR3.Examples.ExampleApps.BatchedGroupExample.Systems
{
    public class SpawnerSystem : IManualSystem
    {
        private IDisposable _sub;
        private IBlueprint _blueprint = new MoveableBlueprint();
            
        public IEntityCollection EntityCollection { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        public SpawnerSystem(IEntityCollection entityCollection, IEntityComponentAccessor entityComponentAccessor)
        {
            EntityCollection = entityCollection;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public void StartSystem()
        { _sub = Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(x => Spawn()); }

        public void Spawn()
        { EntityCollection.Create(EntityComponentAccessor, _blueprint); }

        public void StopSystem()
        { _sub.Dispose(); }
    }
}