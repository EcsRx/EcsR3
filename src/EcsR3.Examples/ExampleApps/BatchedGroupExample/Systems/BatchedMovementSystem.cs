using System;
using System.Numerics;
using SystemsR3.Threading;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Components;
using EcsR3.Systems.Batching;
using EcsR3.Systems.Batching.Convention;
using R3;

namespace EcsR3.Examples.ExampleApps.BatchedGroupExample.Systems
{
    public class BatchedMovementSystem : BatchedMixedSystem<PositionComponent, MovementSpeedComponent>
    {
        public BatchedMovementSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        {}

        protected override Observable<bool> ReactWhen()
        { return Observable.Interval(TimeSpan.FromSeconds(0.5f)).Select(x => true); }

        protected override void Process(int entityId, ref PositionComponent positionComponent, MovementSpeedComponent movementSpeedComponent)
        {
            positionComponent.Position += Vector3.One * movementSpeedComponent.Speed;
        }
    }
}