using System.Numerics;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Playground.Components;

namespace EcsR3.Examples.ExampleApps.Playground.StructBased
{
    public class Struct2Application : BasicLoopApplication
    {
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            base.SetupEntities();
        }

        protected override string Description { get; } =
            "Improved by pre-allocating components and using component type ids with structs";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            for (var i = EntityCollection.Count - 1; i >= 0; i--)
            {
                var entity = EntityCollection[i];
                ref var basicComponent = ref entity.GetComponent<StructComponent>(StructComponent1TypeId);
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;

                ref var basicComponent2 = ref entity.GetComponent<StructComponent2>(StructComponent2TypeId);
                basicComponent2.Value += 10;
                basicComponent2.IsTrue = 1;
            }
        }
    }
}
    