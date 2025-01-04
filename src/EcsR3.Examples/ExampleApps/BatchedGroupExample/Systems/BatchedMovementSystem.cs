using System;
using System.Numerics;
using SystemsR3.Threading;
using EcsR3.Collections;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Components;
using EcsR3.Plugins.Batching.Factories;
using EcsR3.Plugins.Batching.Systems;
using R3;

namespace EcsR3.Examples.ExampleApps.BatchedGroupExample.Systems
{
    public class BatchedMovementSystem : BatchedSystem<PositionComponent, MovementSpeedComponent>
    {
        public BatchedMovementSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager) 
            : base(componentDatabase, componentTypeLookup, batchBuilderFactory, threadHandler, observableGroupManager)
        {}

        protected override Observable<bool> ReactWhen()
        { return Observable.Interval(TimeSpan.FromSeconds(0.5f)).Select(x => true); }

        protected override void Process(int entityId, ref PositionComponent positionComponent, ref MovementSpeedComponent movementSpeedComponent)
        {
            positionComponent.Position += Vector3.One * movementSpeedComponent.Speed;
        }
    }
}