using System.Linq;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Conventions;
using EcsR3.Extensions;
using EcsR3.Tests.Models;
using R3;

namespace EcsR3.Tests.EcsR3.Computeds.Models
{
    public class TestComputedDataFromEntityGroup : ComputedDataFromEntityGroup<double>
    {
        public Subject<Unit> ManuallyRefresh = new();
        
        public TestComputedDataFromEntityGroup(IComputedEntityGroup internalObservableGroup) : base(internalObservableGroup)
        {}

        protected override Observable<Unit> RefreshWhen()
        { return ManuallyRefresh; }

        protected override void UpdateComputedData()
        {
            ComputedData = DataSource
                .Where(x => x.HasComponent<TestComponentThree>())
                .Average(x => x.GetHashCode());
        }
    }
}