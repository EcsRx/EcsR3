using EcsR3.Entities.Accessors;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Components;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.ComputedGroups;
using EcsR3.Groups;
using EcsR3.Infrastructure.Extensions;

namespace EcsR3.Examples.ExampleApps.ComputedGroupExample.Modules
{
    public class ComputedModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            registry.Bind<ILowestHealthComputedGroup>(x => x.ToMethod(y =>
            {
                var entityAccessor = y.Resolve<IEntityComponentAccessor>();
                var namedHealthGroup = y.ResolveComputedEntityGroup(new Group(typeof(HasHealthComponent), typeof(HasNameComponent)));
                return new LowestHealthComputedGroup(entityAccessor, namedHealthGroup);
            }));
        }
    }
}