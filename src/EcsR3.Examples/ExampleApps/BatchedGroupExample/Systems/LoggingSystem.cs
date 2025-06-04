using System;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Components;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems.Reactive;
using R3;

namespace EcsR3.Examples.ExampleApps.BatchedGroupExample.Systems
{
    public class LoggingSystem : IReactToGroupSystem
    {
        public IGroup Group { get; } = new Group(typeof(NameComponent), typeof(PositionComponent));

        public Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup observableGroup)
        { return Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => observableGroup); }
        
        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var nameComponent = entityComponentAccessor.GetComponent<NameComponent>(entity);
            var positionComponent = entityComponentAccessor.GetComponent<PositionComponent>(entity);
            Console.WriteLine($"{nameComponent.Name} - {positionComponent.Position}");
        }

        public void BeforeProcessing()
        {
            Console.SetCursorPosition(0,0);
            Console.Clear();
        }

        public void AfterProcessing() {}
    }
}