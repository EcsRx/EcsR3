using System.Threading.Tasks;
using EcsR3.Collections.Entities;
using Persistity.Flow.Pipelines;

namespace EcsR3.Plugins.Persistence.Pipelines
{
    public interface ILoadEntityCollectionPipeline : IFlowPipeline
    {
        Task<IEntityCollection> Execute();
    }
}