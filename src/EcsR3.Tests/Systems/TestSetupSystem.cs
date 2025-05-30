using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Tests.Models;

namespace EcsR3.Tests.Systems
{
    public class TestSetupSystem : ISetupSystem
    {
        public IGroup Group => new Group(typeof(TestComponentOne));

        public void Setup(IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            var testComponent = entityComponentAccessor.GetComponent<TestComponentOne>(entityId);
            testComponent.Data = "woop";
        }
    }
}