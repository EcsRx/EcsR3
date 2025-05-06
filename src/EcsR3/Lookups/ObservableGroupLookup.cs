using System.Collections.ObjectModel;
using EcsR3.Groups;
using EcsR3.Groups.Observable;

namespace EcsR3.Lookups
{
    public class ObservableGroupLookup : KeyedCollection<LookupGroup, IObservableGroup>
    {
        protected override LookupGroup GetKeyForItem(IObservableGroup item) => item.Group;

        public IObservableGroup GetByIndex(int index) => Items[index];
    }
}