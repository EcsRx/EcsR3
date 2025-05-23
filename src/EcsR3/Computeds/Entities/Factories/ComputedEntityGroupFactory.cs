using EcsR3.Collections.Entity;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Groups;
using EcsR3.Groups.Tracking;

namespace EcsR3.Computeds.Entities.Factories
{
    public class ComputedEntityGroupFactory : IComputedEntityGroupFactory
    {
        public IGroupTrackerFactory GroupTrackerFactory { get; }
        public IEntityCollection EntityCollection { get; }
        
        public ComputedEntityGroupFactory(IGroupTrackerFactory groupTrackerFactory, IEntityCollection entityCollection)
        {
            GroupTrackerFactory = groupTrackerFactory;
            EntityCollection = entityCollection;
        }

        public IComputedEntityGroup Create(LookupGroup group)
        {
            var tracker = GroupTrackerFactory.TrackGroup(group, EntityCollection);
            return new ComputedEntityGroup(group, tracker, EntityCollection);
        }
    }
}