using System;
using System.Collections.Generic;
using System.Linq;
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

        public DisplayLowestHealthSystem(ILowestHealthComputedGroup lowestHealthGroup)
        {
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
            foreach (var entity in allEntities.OrderBy(x => x.GetHealthPercentile()))
            { Console.WriteLine($"{entity.GetName()} - {entity.GetHealthPercentile()}% hp ({entity.GetHealthString()})"); }

            Console.WriteLine();
            
            _lowestHealthGroup.Refresh();
            var position = 1;
            Console.WriteLine(" == Characters With HP < 50% == ");
            foreach (var entity in _lowestHealthGroup.OrderBy(x => x.GetHealthPercentile()))
            { Console.WriteLine($"{position++}) {entity.GetName()} - {entity.GetHealthPercentile()}% hp ({entity.GetHealthString()})"); }
        }
    }
}