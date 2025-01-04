using SystemsR3.Systems;
using EcsR3.Groups.Observable;
using EcsR3.Plugins.GroupBinding.Attributes;

namespace EcsR3.Tests.Plugins.GroupBinding.Handlers.Helpers
{
    public class SystemMissingGroup : ISystem
    {
        [FromGroup()]
        public IObservableGroup ObservableGroupA { get; set; }
    }
}