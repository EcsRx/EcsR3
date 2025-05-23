using R3;
using SystemsR3.Computeds.Conventions;

namespace EcsR3.Computeds.Entities.Conventions
{
    public abstract class ComputedEntityGroupFromEntityGroup<T> : ComputedFromData<T, IComputedEntityGroup>
    {
        protected ComputedEntityGroupFromEntityGroup(IComputedEntityGroup dataSource) : base(dataSource)
        {}

        protected override Observable<Unit> RefreshWhen() => DataSource.OnChanged.Select(x => Unit.Default);
    }
}