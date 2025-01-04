using System.Collections.ObjectModel;
using EcsR3.Entities;

namespace EcsR3.Lookups
{
    public class EntityLookup : KeyedCollection<int, IEntity>
    {
        protected override int GetKeyForItem(IEntity item) => item.Id;

        public IEntity GetByIndex(int index) => Items[index];
    }
}