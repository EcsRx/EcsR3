using R3;
using SystemsR3.Computeds.Conventions;

namespace EcsR3.Computeds.Entities.Conventions
{
    public abstract class ComputedFromEntityGroup<T> : LazyComputedFromData<T, IComputedEntityGroup>
    {
        protected ComputedFromEntityGroup(IComputedEntityGroup dataSource) : base(dataSource)
        {}

        protected override Observable<Unit> RefreshWhen() => DataSource.OnChanged.Select(x => Unit.Default);
    }
}