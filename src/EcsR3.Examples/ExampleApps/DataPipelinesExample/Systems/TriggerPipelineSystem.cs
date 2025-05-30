using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SystemsR3.Attributes;
using SystemsR3.Systems.Conventional;
using SystemsR3.Types;
using EcsR3.Collections;
using EcsR3.Computeds;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.DataPipelinesExample.Components;
using EcsR3.Examples.ExampleApps.DataPipelinesExample.Events;
using EcsR3.Examples.ExampleApps.DataPipelinesExample.Pipelines;
using EcsR3.Extensions;
using EcsR3.Groups;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EcsR3.Examples.ExampleApps.DataPipelinesExample.Systems
{
    [Priority(PriorityTypes.SuperLow)]
    public class TriggerPipelineSystem : IReactToEventSystem<SavePipelineEvent>
    {
        public PostJsonHttpPipeline SaveJsonPipeline { get; }
        public IComputedEntityGroup PlayerGroup { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        public TriggerPipelineSystem(PostJsonHttpPipeline saveJsonPipeline, IComputedEntityGroupRegistry computedEntityGroupRegistry, IEntityComponentAccessor entityComponentAccessor)
        {
            SaveJsonPipeline = saveJsonPipeline;
            EntityComponentAccessor = entityComponentAccessor;
            PlayerGroup = GetPlayerGroup(computedEntityGroupRegistry);
        }

        public async Task TriggerPipeline(PlayerStateComponent playerState)
        {
            var httpResponse = (HttpResponseMessage) await SaveJsonPipeline.Execute(playerState);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            var prettyResponse = MakeDataPretty(responseContent);
            Console.WriteLine($"Server Responded With {prettyResponse}");
        }

        // Feel free to output everything in the JToken if you want, only showing data for simplicity
        public string MakeDataPretty(string jsonData)
        { return JToken.Parse(jsonData)["data"].ToString(Formatting.Indented); }

        public IComputedEntityGroup GetPlayerGroup(IComputedEntityGroupRegistry computedEntityGroupRegistry)
        { return computedEntityGroupRegistry.GetComputedGroup(new Group(typeof(PlayerStateComponent))); }
        
        public void Process(SavePipelineEvent eventData)
        {
            var playerState = EntityComponentAccessor.GetComponent<PlayerStateComponent>(PlayerGroup.Value.Single());
            Task.Run(() => TriggerPipeline(playerState));
        }
    }
}