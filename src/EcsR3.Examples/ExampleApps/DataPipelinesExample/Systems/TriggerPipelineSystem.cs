using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SystemsR3.Attributes;
using SystemsR3.Systems.Conventional;
using SystemsR3.Types;
using EcsR3.Collections;
using EcsR3.Examples.ExampleApps.DataPipelinesExample.Components;
using EcsR3.Examples.ExampleApps.DataPipelinesExample.Events;
using EcsR3.Examples.ExampleApps.DataPipelinesExample.Pipelines;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EcsR3.Examples.ExampleApps.DataPipelinesExample.Systems
{
    [Priority(PriorityTypes.SuperLow)]
    public class TriggerPipelineSystem : IReactToEventSystem<SavePipelineEvent>
    {
        public PostJsonHttpPipeline SaveJsonPipeline { get; }
        public IObservableGroup PlayerGroup { get; }
        
        public TriggerPipelineSystem(PostJsonHttpPipeline saveJsonPipeline, IObservableGroupManager observableGroupManager)
        {
            SaveJsonPipeline = saveJsonPipeline;
            PlayerGroup = GetPlayerGroup(observableGroupManager);
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

        public IObservableGroup GetPlayerGroup(IObservableGroupManager observableGroupManager)
        { return observableGroupManager.GetObservableGroup(new Group(typeof(PlayerStateComponent))); }
        
        public void Process(SavePipelineEvent eventData)
        {
            var playerState = PlayerGroup.Single().GetComponent<PlayerStateComponent>();
            Task.Run(() => TriggerPipeline(playerState));
        }
    }
}