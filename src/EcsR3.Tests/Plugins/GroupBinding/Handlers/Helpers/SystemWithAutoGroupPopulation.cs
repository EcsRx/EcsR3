using SystemsR3.Systems;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using EcsR3.Plugins.GroupBinding.Attributes;
using EcsR3.Systems;
using EcsR3.Tests.Models;
using EcsR3.Attributes;

namespace EcsR3.Tests.Plugins.GroupBinding.Handlers.Helpers
{
    [CollectionAffinity(3)]
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
        [CollectionAffinity(2)]
        public IObservableGroup ObservableGroupAInCollection2;


        [FromComponents(typeof(TestComponentTwo))]
        [CollectionAffinity(5)]
        public IObservableGroup ObservableGroupBInCollection5;

        [FromGroup]
        [CollectionAffinity(7)]
        public IObservableGroup ObservableGroupCInCollection7 { get; set; }

        public int IgnoredField;
    }
}