using SystemsR3.Events;
using SystemsR3.Events.Messages;
using SystemsR3.Executor;
using SystemsR3.Executor.Handlers;
using SystemsR3.Executor.Handlers.Conventional;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Scheduling;
using SystemsR3.Threading;

namespace SystemsR3.Infrastructure.Modules
{
    public class FrameworkModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            registry.Bind<IMessageBroker, MessageBroker>();
            registry.Bind<IEventSystem, EventSystem>();
            registry.Bind<IThreadHandler, DefaultThreadHandler>();
            registry.Bind<IConventionalSystemHandler, ManualSystemHandler>();
            registry.Bind<IConventionalSystemHandler, BasicSystemHandler>();
            registry.Bind<IConventionalSystemHandler, ReactToEventSystemHandler>();
            registry.Bind<IConventionalSystemHandler, ReactiveSystemHandler>();
            registry.Bind<ISystemExecutor, SystemExecutor>();
            registry.Bind<IUpdateScheduler, DefaultUpdateScheduler>();
            registry.Bind<ITimeTracker>(x => x.ToBoundType(typeof(IUpdateScheduler)));
        }
    }
}