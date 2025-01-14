using System;
using EcsR3.Components;
using LazyData.Attributes;

namespace EcsR3.Examples.ExampleApps.DataPipelinesExample.Components
{
    [Persist]
    public class PlayerStateComponent : IComponent
    {
        [PersistData] public int Level { get; set; }
        [PersistData] public string Name { get; set; }
        [PersistData] public TimeSpan PlayTime { get; set; } 
        public string SomeFieldThatWontBePersisted { get; set; }
    }
}