using System.Collections.Generic;
using System.Threading.Tasks;
using EcsR3.Plugins.Persistence.Builders;
using EcsR3.Plugins.Persistence.Transformers;
using EcsR3.Collections.Database;
using Persistity.Core.Serialization;
using Persistity.Endpoints;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps.Types;

namespace EcsR3.Plugins.Persistence.Pipelines
{
    public class DefaultSaveEntityDatabasePipeline : FlowPipeline, ISaveEntityDatabasePipeline
    {
        public ISerializer Serializer { get; }
        public IToEntityDatabaseDataTransformer DataTransformer { get; }
        public ISendDataEndpoint Endpoint { get; }

        public DefaultSaveEntityDatabasePipeline(EcsRxPipelineBuilder pipelineBuilder, ISerializer serializer, IToEntityDatabaseDataTransformer dataTransformer, ISendDataEndpoint endpoint)
        {
            Serializer = serializer;
            DataTransformer = dataTransformer;
            Endpoint = endpoint;

            Steps = BuildSteps(pipelineBuilder);
        }

        public Task Execute(IEntityDatabase entityDatabase)
        { return Execute(entityDatabase, null); }
        
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