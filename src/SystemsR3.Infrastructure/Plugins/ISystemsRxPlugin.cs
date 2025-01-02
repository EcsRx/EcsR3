using System;
using System.Collections.Generic;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Systems;

namespace SystemsR3.Infrastructure.Plugins
{
    public interface ISystemsRxPlugin
    {
        string Name { get; }
        Version Version { get; }

        void SetupDependencies(IDependencyRegistry registry);
        IEnumerable<ISystem> GetSystemsForRegistration(IDependencyResolver resolver);
    }
}