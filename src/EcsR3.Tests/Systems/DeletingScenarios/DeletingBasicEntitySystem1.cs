using EcsR3.Collections.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Tests.Models;
using SystemsR3.Scheduling;

namespace EcsR3.Tests.Systems.DeletingScenarios
{
    public class DeletingBasicEntitySystem1 : IBasicEntitySystem
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();
        
        public IEntityCollection EntityCollection { get; }

        public DeletingBasicEntitySystem1(IEntityCollection entityCollection)
        { EntityCollection = entityCollection; }

        public void Process(IEntityComponentAccessor entityComponentAccessor, int entityId, ElapsedTime elapsedTime)
        { EntityCollection.Remove(entityId); }
    }
}