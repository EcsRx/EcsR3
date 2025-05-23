using EcsR3.Collections;
using EcsR3.Computeds;
using SystemsR3.Systems;
using EcsR3.Plugins.GroupBinding.Attributes;

namespace EcsR3.Tests.Plugins.GroupBinding.Handlers.Helpers
{
    public class SystemMissingGroup : ISystem
    {
        [FromGroup()]
        public IComputedEntityGroupRegistry ObservableEntityGroupA { get; set; }
    }
}