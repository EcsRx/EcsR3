using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using R3;
using SystemsR3.Attributes;
using SystemsR3.Events;
using SystemsR3.Systems;
using SystemsR3.Systems.Conventional;
using SystemsR3.Extensions;

namespace SystemsR3.Executor.Handlers.Conventional
{
    [Priority(6)]
    public class ReactToEventSystemHandler : IConventionalSystemHandler
    {
        private readonly MethodInfo _setupSystemGenericMethodInfo;
        public readonly IEventSystem EventSystem;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;

        public ReactToEventSystemHandler(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
            _setupSystemGenericMethodInfo = typeof(ReactToEventSystemHandler).GetMethod(nameof(SetupSystemGeneric));
        }

        public bool CanHandleSystem(ISystem system)
        { return system.IsReactToEventSystem(); }

        public Type[] GetMatchingInterfaces(ISystem system)
        { return system.GetGenericInterfacesFor(typeof(IReactToEventSystem<>)).ToArray(); }
        
        public IEnumerable<Type> GetEventTypesFromSystem(ISystem system)
        { return system.GetGenericDataTypes(typeof(IReactToEventSystem<>)); }

        public void SetupSystem(ISystem system)
        {
            var matchingInterfaces = GetMatchingInterfaces(system);
            var disposables = new List<IDisposable>();
            foreach (var matchingInterface in matchingInterfaces)
            {
                var eventType = matchingInterface.GetGenericArguments()[0];
                var disposable = (IDisposable)_setupSystemGenericMethodInfo.MakeGenericMethod(eventType).Invoke(this, new object[] { system });
                disposables.Add(disposable);
            }
            _systemSubscriptions.Add(system, new CompositeDisposable(disposables));
        }

        public IDisposable SetupSystemGeneric<T>(IReactToEventSystem<T> system)
        { return EventSystem.Receive<T>().Subscribe(x => system.Process(x)); }
        
        public void DestroySystem(ISystem system)
        { _systemSubscriptions.RemoveAndDispose(system); }

        public void Dispose()
        {
            _systemSubscriptions.Values.DisposeAll();
            _systemSubscriptions.Clear();
        }
    }
}
