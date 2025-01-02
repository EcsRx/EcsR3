﻿using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsR3.Collections;
using EcsR3.Collections.Entity;
using EcsR3.Components;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Performance.Components.Specific;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using SystemsR3.Extensions;

namespace EcsR3.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Groups")]
    public class MultipleObservableGroupsAddAndRemoveBenchmark : EcsR3Benchmark
    {
        private IComponent[] _availableComponents;
        private Type[] _availableComponentTypes;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        
        [Params(1000)]
        public int EntityCount;
        
        [Params(1, 10, 25)]
        public int ObservableGroups;

        private IEntityCollection _collection;

        public MultipleObservableGroupsAddAndRemoveBenchmark() : base()
        {
            var componentNamespace = typeof(Component1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
            
            _collection = EntityDatabase.GetCollection();
        }

        public override void Setup()
        {
            var componentsPerGroup = _availableComponentTypes.Length / ObservableGroups;
            for (var i = 0; i < ObservableGroups; i++)
            {
                var componentsToTake = _availableComponentTypes
                    .Skip(i*(componentsPerGroup))
                    .Take(componentsPerGroup)
                    .ToArray();
                                
                var group = new Group(componentsToTake);
                ObservableGroupManager.GetObservableGroup(group);
            }
            
            _availableComponents = _availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
        }

        public override void Cleanup()
        {
            _collection.RemoveAllEntities();
            var manager = (ObservableGroupManager as ObservableGroupManager);
            var allObservableGroups = manager._observableGroups.ToArray();
            allObservableGroups.DisposeAll();
            manager._observableGroups.Clear();
        }

        [Benchmark]
        public void MultipleObservableGroupAddRemove()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = _collection.CreateEntity();
                entity.AddComponents(_availableComponents);
                entity.RemoveAllComponents();
            }
        }
    }
}