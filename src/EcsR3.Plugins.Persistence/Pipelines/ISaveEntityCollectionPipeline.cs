using System.Threading.Tasks;
using EcsR3.Collections.Entity;
using Persistity.Flow.Pipelines;

namespace EcsR3.Plugins.Persistence.Pipelines
{
    public interface ISaveEntityCollectionPipeline : IFlowPipeline
    {
        Task Execute(IEntityCollection collection);
    }
}