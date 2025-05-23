using System.Collections.Generic;
using System.Net.Http;
using EcsR3.Plugins.Persistence.Builders;
using EcsR3.Plugins.Persistence.Pipelines;
using Persistity.Endpoints.Http;
using Persistity.Flow.Steps.Types;
using Persistity.Serializers.LazyData.Json;

namespace EcsR3.Examples.ExampleApps.DataPipelinesExample.Pipelines
{
    public class PostJsonHttpPipeline : EcsRxBuiltPipeline
    {
        public PostJsonHttpPipeline(EcsRxPipelineBuilder pipelineBuilder) : base(pipelineBuilder)
        { }

        protected override IEnumerable<IPipelineStep> BuildSteps(EcsRxPipelineBuilder builder)
        {
            return builder.StartFromInput()
                .SerializeWith<JsonSerializer>(false)
                .ThenSendTo(new HttpSendEndpoint("https://postman-echo.com/post", HttpMethod.Post))
                .BuildSteps();
        }
    }
}