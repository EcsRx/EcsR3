using System.Linq;
using EcsR3.Plugins.Persistence.Data;
using EcsR3.Entities;

namespace EcsR3.Plugins.Persistence.Transformers
{
    public class ToEntityDataTransformer : IToEntityDataTransformer
    {
        public object Transform(object original)
        {
            var entity = (IEntity)original;
            return new EntityData
            {
                EntityId = entity.Id,
                Components = entity.Components.ToList()
            };
        }
    }
}