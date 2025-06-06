using System.Linq;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Conventions;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Tests.Models;
using R3;

namespace EcsR3.Tests.EcsR3.Computeds.Models
{
    public class TestComputedFromEntityGroup : ComputedFromEntityGroup<double>
    {
        public Subject<Unit> ManuallyRefresh = new();
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        public TestComputedFromEntityGroup(IEntityComponentAccessor entityComponentAccessor,
            IComputedEntityGroup internalComputedEntityGroup) : base(internalComputedEntityGroup)
        {
            EntityComponentAccessor = entityComponentAccessor;
        }

        protected override Observable<Unit> RefreshWhen()
        { return ManuallyRefresh; }

        protected override bool UpdateComputedData()
        {
            ComputedData = DataSource
                .Where(EntityComponentAccessor.HasComponent<TestComponentThree>)
                .Average(x => x.GetHashCode());

            return true;
        }
    }
}