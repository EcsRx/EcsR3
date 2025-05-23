using EcsR3.Entities;
using EcsR3.Extensions;
using EcsR3.Tests.Models;
using R3;

namespace EcsR3.Tests.EcsR3.Computeds.Models
{
    /*
    public class TestComputedGroup : ComputedGroup
    {
        public Subject<bool> ManuallyRefresh = new Subject<bool>();
        
        public TestComputedGroup(IObservableGroup internalObservableGroup) : base(internalObservableGroup)
        { }

        public override Observable<bool> RefreshWhen()
        { return ManuallyRefresh; }
        
        public override bool IsEntityApplicable(IEntity entity)
        { return entity.HasComponent<TestComponentOne>(); }
    }*/
}