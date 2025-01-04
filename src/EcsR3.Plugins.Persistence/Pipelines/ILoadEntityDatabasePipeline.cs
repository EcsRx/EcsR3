using System.Threading.Tasks;
using EcsR3.Collections.Database;
using Persistity.Flow.Pipelines;

namespace EcsR3.Plugins.Persistence.Pipelines
{
    public interface ILoadEntityDatabasePipeline : IFlowPipeline
    {
        Task<IEntityDatabase> Execute();
    }
}