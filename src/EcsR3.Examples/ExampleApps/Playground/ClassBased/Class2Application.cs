using System.Numerics;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Playground.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.Playground.ClassBased
{
    public class Class2Application : BasicLoopApplication
    {
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(ClassComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(ClassComponent2TypeId, EntityCount);
            base.SetupEntities();
        }

        protected override string Description { get; } =
            "Improved by pre-allocating components and using component type ids";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<ClassComponent>();
            entity.AddComponent<ClassComponent2>();
        }

        protected override void RunProcess()
        {
            foreach(var entity in EntityCollection)
            {   
                var basicComponent = entity.GetComponent<ClassComponent>(ClassComponent1TypeId);
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
                
                var basicComponent2 = entity.GetComponent<ClassComponent2>(ClassComponent2TypeId);
                basicComponent2.Value += 10;
                basicComponent2.IsTrue = true;
            }
        }
    }
}