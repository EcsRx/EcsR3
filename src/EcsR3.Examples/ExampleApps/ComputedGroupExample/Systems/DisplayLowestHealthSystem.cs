using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections.Entities;
using EcsR3.Entities.Accessors;
using SystemsR3.Extensions;
using SystemsR3.Systems.Conventional;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.ComputedGroups;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Extensions;
using R3;

namespace EcsR3.Examples.ExampleApps.ComputedGroupExample.Systems
{
    public class DisplayLowestHealthSystem : IManualSystem
    {
        private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

        private readonly ILowestHealthComputedGroup _lowestHealthGroup;
        private readonly IEntityComponentAccessor _entityComponentAccessor;
        
        public DisplayLowestHealthSystem(IEntityComponentAccessor entityComponentAccessor, ILowestHealthComputedGroup lowestHealthGroup)
        {
            _entityComponentAccessor = entityComponentAccessor;
            _lowestHealthGroup = lowestHealthGroup;
        }

        public void StartSystem()
        { Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(x => UpdateListings()).AddTo(_subscriptions); }
        
        public void StopSystem()
        { _subscriptions.DisposeAll(); }

        public void UpdateListings()
        {
            Console.SetCursorPosition(0,0);
            Console.Clear();

            var allEntities = (_lowestHealthGroup as LowestHealthComputedGroup).DataSource;
            Console.WriteLine(" == All Characters HP == ");
            foreach (var entityId in allEntities.OrderBy(_entityComponentAccessor.GetHealthPercentile))
            { Console.WriteLine($"{_entityComponentAccessor.GetName(entityId)} - {_entityComponentAccessor.GetHealthPercentile(entityId)}% hp ({_entityComponentAccessor.GetHealthString(entityId)})"); }

            Console.WriteLine();
            
            _lowestHealthGroup.Refresh();
            var position = 1;
            Console.WriteLine(" == Characters With HP < 50% == ");
            foreach (var entityId in _lowestHealthGroup.OrderBy(_entityComponentAccessor.GetHealthPercentile))
            { Console.WriteLine($"{position++}) {_entityComponentAccessor.GetName(entityId)} - {_entityComponentAccessor.GetHealthPercentile(entityId)}% hp ({_entityComponentAccessor.GetHealthString(entityId)})"); }
        }
    }
}