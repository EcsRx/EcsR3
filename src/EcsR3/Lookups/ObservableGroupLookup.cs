using System.Collections.ObjectModel;
using EcsR3.Groups.Observable;

namespace EcsR3.Lookups
{
    public class ObservableGroupLookup : KeyedCollection<ObservableGroupToken, IObservableGroup>
    {
        protected override ObservableGroupToken GetKeyForItem(IObservableGroup item) => item.Token;

        public IObservableGroup GetByIndex(int index) => Items[index];
    }
}