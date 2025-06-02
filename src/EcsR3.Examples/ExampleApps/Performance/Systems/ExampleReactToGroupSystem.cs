using System;
using System.Threading;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.Performance.Components;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Reactive;
using R3;

namespace EcsR3.Examples.ExampleApps.Performance.Systems
{
    public class ExampleReactToGroupSystem : IReactToGroupSystem
    {
        public IGroup Group { get; } = new Group(typeof(SimpleReadComponent), typeof(SimpleWriteComponent));
        
        public Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup observableGroup)
        { return Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => observableGroup); }

        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var readComponent = entityComponentAccessor.GetComponent<SimpleReadComponent>(entity);
            var writeComponent = entityComponentAccessor.GetComponent<SimpleWriteComponent>(entity);
            writeComponent.WrittenValue = readComponent.StartingValue;
            Thread.Sleep(1); // Just to pretend there is something complex happening
        }
    }
}