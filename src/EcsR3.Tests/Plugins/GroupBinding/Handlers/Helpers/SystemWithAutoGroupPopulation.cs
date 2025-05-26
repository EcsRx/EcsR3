using EcsR3.Computeds.Entities;
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
        public IComputedEntityGroup ComputedEntityGroupA { get; set; }
        
        [FromGroup(typeof(TestGroupA))]
        public IComputedEntityGroup IgnoredComputedEntityGroup { get; }
        
        [FromComponents(typeof(TestComponentTwo))]
        public IComputedEntityGroup ComputedEntityGroupB { get; set; }

        public int IgnoredProperty { get; set; }

        [FromGroup]
        public IComputedEntityGroup ComputedEntityGroupC;

        [FromGroup(typeof(TestGroupA))]
        public IComputedEntityGroup ComputedEntityGroupAInCollection2;


        [FromComponents(typeof(TestComponentTwo))]
        public IComputedEntityGroup ComputedEntityGroupBInCollection5;

        [FromGroup]
        public IComputedEntityGroup ComputedEntityGroupCInCollection7 { get; set; }

        public int IgnoredField;
    }
}