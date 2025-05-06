using System.Threading.Tasks;
using EcsR3.Collections.Entity;
using Persistity.Flow.Pipelines;

namespace EcsR3.Plugins.Persistence.Pipelines
{
    public interface ILoadEntityCollectionPipeline : IFlowPipeline
    {
        Task<IEntityCollection> Execute();
    }
}