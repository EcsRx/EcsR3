using System.Collections.ObjectModel;
using EcsR3.Collections.Entity;
using EcsR3.Collections;

namespace EcsR3.Lookups
{
    public class CollectionLookup : KeyedCollection<int, IEntityCollection>
    {
        protected override int GetKeyForItem(IEntityCollection item) => item.Id;

        public IEntityCollection GetByIndex(int index) => Items[index];
    }
}