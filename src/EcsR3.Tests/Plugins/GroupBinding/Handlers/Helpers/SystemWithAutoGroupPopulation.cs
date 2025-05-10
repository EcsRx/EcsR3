using SystemsR3.Systems;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using EcsR3.Plugins.GroupBinding.Attributes;
using EcsR3.Systems;
using EcsR3.Tests.Models;

namespace EcsR3.Tests.Plugins.GroupBinding.Handlers.Helpers
{
    public class SystemWithAutoGroupPopulation : ISystem, IGroupSystem
    {
        public IGroup Group => new TestGroupA();
        
        [FromGroup(typeof(TestGroupA))]
        public IObservableGroup ObservableGroupA { get; set; }
        
        [FromGroup(typeof(TestGroupA))]
        public IObservableGroup IgnoredObservableGroup { get; }
        
        [FromComponents(typeof(TestComponentTwo))]
        public IObservableGroup ObservableGroupB { get; set; }

        public int IgnoredProperty { get; set; }

        [FromGroup]
        public IObservableGroup ObservableGroupC;

        [FromGroup(typeof(TestGroupA))]
        public IObservableGroup ObservableGroupAInCollection2;


        [FromComponents(typeof(TestComponentTwo))]
        public IObservableGroup ObservableGroupBInCollection5;

        [FromGroup]
        public IObservableGroup ObservableGroupCInCollection7 { get; set; }

        public int IgnoredField;
    }
}