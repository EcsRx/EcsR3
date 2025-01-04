using SystemsR3.Scheduling;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.DataPipelinesExample.Components;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;

namespace EcsR3.Examples.ExampleApps.DataPipelinesExample.Systems
{
    public class PlayerStateUpdaterEntitySystem : IBasicEntitySystem
    {
        public IGroup Group { get; } = new Group(typeof(PlayerStateComponent));

        public void Process(IEntity entity, ElapsedTime elapsedTime)
        {
            var playerState = entity.GetComponent<PlayerStateComponent>();
            playerState.PlayTime += elapsedTime.DeltaTime;
        }
    }
}