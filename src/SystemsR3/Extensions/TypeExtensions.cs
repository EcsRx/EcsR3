using System;
using System.Collections.Generic;
using System.Linq;
using SystemsR3.Systems;

namespace SystemsR3.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetMatchingInterfaceGenericTypes(this Type type, Type genericType)
        {
            return type
                .GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericType);
        }
    }
}