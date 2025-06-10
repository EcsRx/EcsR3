using System;
using System.Linq;
using System.Reflection;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Systems;

namespace SystemsR3.Infrastructure.Extensions
{
    public static class IDependencyRegistrySystemExtensions
    {
        private static readonly Type SystemType = typeof(ISystem);     
        
        /// <summary>
        /// This will bind any ISystem implementations that are found within the namespaces provided
        /// </summary>
        /// <remarks>
        /// It is also advised you wrap this method with your own conventions like BindAllSystemsWithinApplicationScope does.
        /// </remarks>
        /// <param name="registry">The registry to bind on</param>
        /// <param name="namespaces">The namespaces to be scanned for implementations</param>
        public static void BindAllSystemsInNamespaces(this IDependencyRegistry registry, params string[] namespaces)
        {
            var applicationAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var applicableSystems = applicationAssemblies.SelectMany(x => x.GetTypes())
                .Where(x =>
                {                   
                    if(x.IsInterface || x.IsAbstract)
                    { return false; }
                    
                    if(string.IsNullOrEmpty(x.Namespace))
                    { return false; }   
                    
                    if(!SystemType.IsAssignableFrom(x))
                    { return false; }

                    return namespaces.Any(namespaceToVerify => x.Namespace.Contains(namespaceToVerify));
                })
                .ToList();

            if(!applicableSystems.Any())
            { return; }

            foreach (var applicableSystemType in applicableSystems)
            { BindSystem(registry, applicableSystemType); }
        }
        
        /// <summary>
        /// This will bind any ISystem implementations that are found within the assembly provided
        /// </summary>
        /// <remarks>
        /// This can save you time but is not advised in most cases
        /// </remarks>
        /// <param name="registry">The dependency registry to use</param>
        /// <param name="assemblies">The assemblies to scan for systems</param>
        public static void BindAllSystemsInAssemblies(this IDependencyRegistry registry, params Assembly[] assemblies)
        {           
            var systemType = typeof(ISystem);           
            
            var applicableSystems = assemblies.SelectMany(x => x.GetTypes())
                .Where(x =>
                {                   
                    if(x.IsInterface || x.IsAbstract)
                    { return false; }
                                        
                    return systemType.IsAssignableFrom(x);
                })
                .ToList();

            if(!applicableSystems.Any())
            { return; }

            foreach (var applicableSystemType in applicableSystems)
            { BindSystem(registry, applicableSystemType); }
        }

        /// <summary>
        /// Binds a system against ISystem and with its name for direct access
        /// </summary>
        /// <param name="registry">The registry to use</param>
        /// <typeparam name="T">The system type</typeparam>
        /// <remarks>You can bind your systems however you want, but this provides a consistent way of doing it</remarks>
        public static void BindSystem<T>(this IDependencyRegistry registry) where T : ISystem
        {
            var systemType = typeof(T);
            BindSystem(registry, systemType);
        }

        /// <summary>
        /// Internal conventional binder, which binds all systems as singletons against ISystem with its name
        /// as a unique selector for direct access if needed
        /// </summary>
        /// <param name="registry">The registry to use</param>
        /// <param name="systemType">The system type to bind</param>
        private static void BindSystem(this IDependencyRegistry registry, Type systemType)
        {
            var bindingConfiguration = new BindingConfiguration
            {
                AsSingleton = true,
                WithName = systemType.Name
            };
            registry.Bind(SystemType, systemType, bindingConfiguration);
        }
    }
}