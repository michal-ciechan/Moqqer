using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MoqqerNamespace.Helpers
{
    internal static class TypeHelpers
    {
        public static ConstructorInfo GetDefaultCtor(this Type type)
        {
            return type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, Type.EmptyTypes, null);
        }

        public static IEnumerable<MethodInfo> GetMockableMethods(this Type type)
        {
            return type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(
                    x => x.ReturnType.IsInterface 
                    && !x.IsGenericMethod 
                    && !x.IsGenericMethodDefinition 
                    && x.IsVirtual);
        }

        public static ConstructorInfo FindConstructor(this Type type)
        {
            var ctors = type.GetConstructors();

            var potentialCtors = ctors.Where(c => c.GetParameters().All(p => !p.ParameterType.IsValueType)).ToList();

            if (potentialCtors.Count == 0)
                throw new MoqqerException($"Could not find any possible constructors for type: {type.Name}");

            var maxParams = potentialCtors.Max(c => c.GetParameters().Length);

            return potentialCtors.First(c => c.GetParameters().Length == maxParams);
        }

        public static MethodInfo GetGenericMethod(this Type type, string methodName)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var methods = type
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == methodName)
                .ToList();

            if (methods.Count == 0)
                throw new Exception($"Could not find any methods named '{methodName}' on Type '{type.Name}'");

            var genericMethods = methods
                .Select(m => new
                {
                    Method = m,
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Args.Length > 0)
                .Select(x => x.Method)
                .ToList();

            if (genericMethods.Count == 0)
                throw new Exception($"Could not find any generic methods named '{methodName}' on Type '{type.Name}'");

            if (genericMethods.Count > 1)
                throw new Exception($"Found multiple generic methods named '{methodName}' on Type '{type.Name}'");

            return genericMethods[0];
        }
    }
}
