using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Conventions;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Extensions;

namespace EcsR3.Examples.ExampleApps.ComputedGroupExample.ComputedGroups
{
    public class LowestHealthComputedGroup : ComputedEntityGroupFromEntityGroup, ILowestHealthComputedGroup
    {
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        public LowestHealthComputedGroup(IEntityComponentAccessor entityComponentAccessor, IComputedEntityGroup computedEntityGroup) 
            : base(entityComponentAccessor, computedEntityGroup)
        {
            EntityComponentAccessor = entityComponentAccessor;
        }

        public override bool IsEntityValid(Entity entity)
        {
            var healthPercentage = EntityComponentAccessor.GetHealthPercentile(entity);
            return healthPercentage < 50;
        }
    }
}