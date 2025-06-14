﻿using System;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.HelloWorldExample.Components;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Reactive;
using R3;

namespace EcsR3.Examples.ExampleApps.HelloWorldExample.Systems
{
    public class TalkingGroupSystem : IReactToGroupSystem
    {
        public IGroup Group => new Group(typeof(CanTalkComponent));

        public Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup observableGroup)
        { return Observable.Interval(TimeSpan.FromSeconds(2)).Select(x => observableGroup); }

        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var canTalkComponent = entityComponentAccessor.GetComponent<CanTalkComponent>(entity);
            Console.WriteLine($"Entity says '{canTalkComponent.Message}' @ {DateTime.Now}");
        }
    }
}