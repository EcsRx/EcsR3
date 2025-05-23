using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Registries;

namespace EcsR3.Plugins.Batching.Accessors
{
    public class AccessorToken
    {
        public int[] ComponentTypeIds { get; }
        public IComputedEntityGroup ObservableGroup { get; }

        public AccessorToken(int[] componentTypeIds, IComputedEntityGroup observableGroup)
        {
            ComponentTypeIds = componentTypeIds;
            ObservableGroup = observableGroup;
        }
    }
}