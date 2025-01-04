using System.Collections.Generic;
using EcsR3.Plugins.Persistence.Builders;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps.Types;

namespace EcsR3.Plugins.Persistence.Pipelines
{
    public abstract class EcsRxBuiltPipeline : FlowPipeline
    {
        public EcsRxBuiltPipeline(EcsRxPipelineBuilder pipelineBuilder)
        {
            Steps = BuildSteps(pipelineBuilder);
        }

        protected abstract IEnumerable<IPipelineStep> BuildSteps(EcsRxPipelineBuilder builder);
    }
}