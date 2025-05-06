using System.Collections.Generic;
using EcsR3.Collections.Entity;
using EcsR3.Entities;

namespace EcsR3.Groups.Observable
{
    public class ObservableGroupConfiguration
    {
        public LookupGroup Group { get; set; }
        public IEnumerable<INotifyingCollection> NotifyingCollections { get; set; }
        public IEnumerable<IEntity> InitialEntities { get; set; }
    }
}