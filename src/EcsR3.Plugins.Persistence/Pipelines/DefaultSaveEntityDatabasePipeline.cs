using System.Collections.Generic;
using System.Threading.Tasks;
using EcsR3.Collections.Entities;
using EcsR3.Plugins.Persistence.Builders;
using EcsR3.Plugins.Persistence.Transformers;
using Persistity.Core.Serialization;
using Persistity.Endpoints;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps.Types;

namespace EcsR3.Plugins.Persistence.Pipelines
{
    public class DefaultSaveEntityCollectionPipeline : FlowPipeline, ISaveEntityCollectionPipeline
    {
        public ISerializer Serializer { get; }
        public IToEntityCollectionDataTransformer DataTransformer { get; }
        public ISendDataEndpoint Endpoint { get; }

        public DefaultSaveEntityCollectionPipeline(EcsRxPipelineBuilder pipelineBuilder, ISerializer serializer, IToEntityCollectionDataTransformer dataTransformer, ISendDataEndpoint endpoint)
        {
            Serializer = serializer;
            DataTransformer = dataTransformer;
            Endpoint = endpoint;

            Steps = BuildSteps(pipelineBuilder);
        }

        public Task Execute(IEntityCollection entityCollection)
        { return Execute(entityCollection, null); }
        
        protected IEnumerable<IPipelineStep> BuildSteps(EcsRxPipelineBuilder builder)
        {
            return builder
                .StartFromInput()
                .TransformWith(DataTransformer)
                .SerializeWith(Serializer)
                .ThenSendTo(Endpoint)
                .BuildSteps();
        }
    }
}