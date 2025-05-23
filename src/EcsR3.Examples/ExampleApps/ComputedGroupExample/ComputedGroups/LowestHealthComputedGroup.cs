using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Extensions;
using R3;

namespace EcsR3.Examples.ExampleApps.ComputedGroupExample.ComputedGroups
{
    /*
    public class LowestHealthComputedGroup : ComputedGroup, ILowestHealthComputedGroup
    {
        public LowestHealthComputedGroup(IObservableGroup internalObservableGroup) : base(internalObservableGroup)
        {}

        public override Observable<bool> RefreshWhen()
        { return Observable.Interval(TimeSpan.FromMilliseconds(100)).Select(x => true); }

        public override bool IsEntityApplicable(IEntity entity)
        {
            var healthPercentage = entity.GetHealthPercentile();
            return healthPercentage < 50;
        }
        
        public override IEnumerable<IEntity> PostProcess(IEnumerable<IEntity> entities)
        { return entities.OrderBy(x => x.GetHealthPercentile()); }

        
    }*/
}