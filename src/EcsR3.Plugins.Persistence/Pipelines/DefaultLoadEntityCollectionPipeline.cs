using System.Collections.Generic;
using System.Threading.Tasks;
using EcsR3.Collections.Entities;
using EcsR3.Plugins.Persistence.Builders;
using EcsR3.Plugins.Persistence.Data;
using EcsR3.Plugins.Persistence.Transformers;
using Persistity.Core.Serialization;
using Persistity.Endpoints;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps.Types;

namespace EcsR3.Plugins.Persistence.Pipelines
{
    public class DefaultLoadEntityCollectionPipeline : FlowPipeline, ILoadEntityCollectionPipeline
    {
        public IDeserializer Deserializer { get; }
        public IFromEntityCollectionDataTransformer DataTransformer { get; }
        public IReceiveDataEndpoint Endpoint { get; }

        public DefaultLoadEntityCollectionPipeline(EcsRxPipelineBuilder pipelineBuilder, IDeserializer deserializer, IFromEntityCollectionDataTransformer dataTransformer, IReceiveDataEndpoint endpoint)
        {
            Deserializer = deserializer;
            DataTransformer = dataTransformer;
            Endpoint = endpoint;

            Steps = BuildSteps(pipelineBuilder);
        }

        public async Task<IEntityCollection> Execute()
        { return (IEntityCollection) await Execute(null).ConfigureAwait(false); }

        protected IEnumerable<IPipelineStep> BuildSteps(EcsRxPipelineBuilder builder)
        {
            return builder
                .StartFrom(Endpoint)
                .DeserializeWith(Deserializer, typeof(EntityCollectionData))
                .TransformWith(DataTransformer)
                .BuildSteps();
        }
    }
}