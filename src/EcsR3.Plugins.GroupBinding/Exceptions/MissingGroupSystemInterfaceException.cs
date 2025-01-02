using System;
using System.Reflection;
using SystemsR3.Systems;

namespace EcsR3.Plugins.GroupBinding.Exceptions
{
    public class MissingGroupSystemInterfaceException : Exception
    {
        public ISystem System { get; }
        public MemberInfo Member { get; }

        public MissingGroupSystemInterfaceException(ISystem system, MemberInfo member) 
            : base($"{member.Name} GroupFrom attribute cannot find an IGroupSystem on {system.GetType().Name}")
        {
            System = system;
            Member = member;
        }
    }
}