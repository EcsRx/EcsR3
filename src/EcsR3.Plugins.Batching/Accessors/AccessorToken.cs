using EcsR3.Groups.Observable;

namespace EcsR3.Plugins.Batching.Accessors
{
    public class AccessorToken
    {
        public int[] ComponentTypeIds { get; }
        public IObservableGroup ObservableGroup { get; }

        public AccessorToken(int[] componentTypeIds, IObservableGroup observableGroup)
        {
            ComponentTypeIds = componentTypeIds;
            ObservableGroup = observableGroup;
        }
    }
}