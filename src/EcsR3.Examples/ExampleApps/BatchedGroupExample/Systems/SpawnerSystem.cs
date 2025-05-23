using System;
using SystemsR3.Systems.Conventional;
using EcsR3.Blueprints;
using EcsR3.Collections.Entity;
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

        public SpawnerSystem(IEntityCollection entityCollection)
        { EntityCollection = entityCollection; }

        public void StartSystem()
        { _sub = Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(x => Spawn()); }

        public void Spawn()
        { EntityCollection.Create(_blueprint); }

        public void StopSystem()
        { _sub.Dispose(); }
    }
}