using System;
using System.Linq;
using EcsR3.Computeds;
using EcsR3.Extensions;
using EcsR3.Groups.Observable;
using EcsR3.Tests.Models;
using R3;

namespace EcsR3.Tests.EcsRx.Computeds.Models
{
    public class TestComputedFromGroup : ComputedFromGroup<double>
    {
        public Subject<bool> ManuallyRefresh = new Subject<bool>();
        
        public TestComputedFromGroup(IObservableGroup internalObservableGroup) : base(internalObservableGroup)
        {}

        public override Observable<bool> RefreshWhen()
        { return ManuallyRefresh; }

        public override double Transform(IObservableGroup observableGroup)
        { return observableGroup.Value.Where(x => x.HasComponent<TestComponentThree>()).Average(x => x.GetHashCode()); }
    }
}