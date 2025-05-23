using System.Numerics;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Playground.Components;

namespace EcsR3.Examples.ExampleApps.Playground.StructBased
{
    /// <summary>
    /// 
    /// </summary>
    public class Struct1Application : BasicLoopApplication
    {
        protected override string Description { get; } = "Simplest possible approach but with structs";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            foreach (var entity in EntityCollection)
            {
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