using System;
using SystemsR3.Systems;

namespace SystemsR3.Exceptions
{
    public class SystemAlreadyRegisteredException : Exception
    {
        public SystemAlreadyRegisteredException(ISystem system) : base($"System [{system.GetType().Name}] has already been registered")
        {
        }
    }
}