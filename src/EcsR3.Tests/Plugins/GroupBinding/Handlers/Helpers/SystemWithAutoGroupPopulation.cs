using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Registries;
using SystemsR3.Systems;
using EcsR3.Groups;
using EcsR3.Plugins.GroupBinding.Attributes;
using EcsR3.Systems;
using EcsR3.Tests.Models;

namespace EcsR3.Tests.Plugins.GroupBinding.Handlers.Helpers
{
    public class SystemWithAutoGroupPopulation : ISystem, IGroupSystem
    {
        public IGroup Group => new TestGroupA();
        
        [FromGroup(typeof(TestGroupA))]
        public IComputedEntityGroup ObservableGroupA { get; set; }
        
        [FromGroup(typeof(TestGroupA))]
        public IComputedEntityGroup IgnoredObservableGroup { get; }
        
        [FromComponents(typeof(TestComponentTwo))]
        public IComputedEntityGroup ObservableGroupB { get; set; }

        public int IgnoredProperty { get; set; }

        [FromGroup]
        public IComputedEntityGroup ObservableGroupC;

        [FromGroup(typeof(TestGroupA))]
        public IComputedEntityGroup ObservableGroupAInCollection2;


        [FromComponents(typeof(TestComponentTwo))]
        public IComputedEntityGroup ObservableGroupBInCollection5;

        [FromGroup]
        public IComputedEntityGroup ObservableGroupCInCollection7 { get; set; }

        public int IgnoredField;
    }
}