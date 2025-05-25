using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Conventions;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Extensions;
using R3;

namespace EcsR3.Examples.ExampleApps.ComputedGroupExample.ComputedGroups
{
    public class LowestHealthComputedGroup : ComputedEntityGroupFromEntityGroup, ILowestHealthComputedGroup
    {
        public LowestHealthComputedGroup(IComputedEntityGroup computedEntityGroup) : base(computedEntityGroup)
        {}

        public override bool IsEntityValid(IEntity entity)
        {
            var healthPercentage = entity.GetHealthPercentile();
            return healthPercentage < 50;
        }
    }
}