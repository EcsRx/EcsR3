using System.Collections.Generic;
using System.Threading.Tasks;
using EcsR3.Plugins.Persistence.Builders;
using EcsR3.Plugins.Persistence.Data;
using EcsR3.Plugins.Persistence.Transformers;
using EcsR3.Collections.Database;
using Persistity.Core.Serialization;
using Persistity.Endpoints;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps.Types;

namespace EcsR3.Plugins.Persistence.Pipelines
{
    public class DefaultLoadEntityDatabasePipeline : FlowPipeline, ILoadEntityDatabasePipeline
    {
        public IDeserializer Deserializer { get; }
        public IFromEntityDatabaseDataTransformer DataTransformer { get; }
        public IReceiveDataEndpoint Endpoint { get; }

        public DefaultLoadEntityDatabasePipeline(EcsRxPipelineBuilder pipelineBuilder, IDeserializer deserializer, IFromEntityDatabaseDataTransformer dataTransformer, IReceiveDataEndpoint endpoint)
        {
            Deserializer = deserializer;
            DataTransformer = dataTransformer;
            Endpoint = endpoint;

            Steps = BuildSteps(pipelineBuilder);
        }

        public async Task<IEntityDatabase> Execute()
        { return (IEntityDatabase) await Execute(null).ConfigureAwait(false); }

        protected IEnumerable<IPipelineStep> BuildSteps(EcsRxPipelineBuilder builder)
        {
            return builder
                .StartFrom(Endpoint)
                .DeserializeWith(Deserializer, typeof(EntityDatabaseData))
                .TransformWith(DataTransformer)
                .BuildSteps();
        }
    }
}