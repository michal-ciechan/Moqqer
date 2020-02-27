using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MoqqerNamespace.Helpers
{
    internal static class TypeExtensions
    {
	    public static object GetDefaultInstance(this Type type)
	    {
		    return type.IsValueType ? Activator.CreateInstance(type) : null;
	    }

		public static IEnumerable<Type> FlattenInheritance(this Type type)
        {
            yield return type;

            foreach (var subType in type.GetInterfaces().SelectMany(x => x.FlattenInheritance()).Distinct())
            {
                yield return subType;
            }
        }

        public static ConstructorInfo GetDefaultCtor(this Type type)
        {
            return type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, Type.EmptyTypes, null);
        }
        public static bool HasDefaultCtor(this Type type)
        {
            return GetDefaultCtor(type) != null;
        }

        public static IEnumerable<MethodInfo> GetMockableMethods(this Type type, Predicate<Type> canInject)
        {
            return type
                .FlattenInheritance()
                .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |  BindingFlags.FlattenHierarchy))
                .Where(x =>
                {
                    try
                    {
                        return x.IsMethodMockable() || canInject(x.ReturnType);
                    }
                    catch (Exception e)
                    {
                        throw new MoqqerException($"Exception happened while trying to check if Method '{x.Describe()}' is Mockable. See inner exception for more details.", e);
                    }
                });
        }

        private static bool IsMethodMockable(this MethodInfo x)
        {
            return x.ReturnType.IsInterface
                   && !x.IsGenericMethod 
                   && !x.IsGenericMethodDefinition 
                   && x.IsVirtual;
        }

        public static ConstructorInfo FindConstructor(this Type type, Predicate<Type> canInject)
        {
            var ctors = type.GetConstructors();

            var potentialCtors = ctors
                .Where(c => c.GetParameters()
                    .Select(p => p.ParameterType)
                    .All(p => p.IsInjectable(canInject) ||
                              p.IsInjectableFunc(canInject)))
                .ToList();

            if (potentialCtors.Count == 0)
                throw new MoqqerException($"Could not find any possible constructors for type: {type.Describe()}");

            return potentialCtors.OrderByDescending(x => x.GetParameters().Length).First();

        }

        internal static bool IsFunc(this Type type)
        {
            if (!type.IsGenericType)
                return false;

            return type.GetGenericTypeDefinition() == typeof(Func<>);
        }

        internal static bool IsInjectableFunc(this Type type, Predicate<Type> canInject)
        {
            if (!type.IsFunc())
                return false;

            var returnType = type.GetGenericArguments().First();

            return returnType.IsInjectable(canInject);
        }

        public static string Describe(this Type type)
        {
            if (type == typeof(void)) return "void";
            if (type == typeof(object)) return "object";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(byte)) return "byte";
            if (type == typeof(short)) return "short";
            if (type == typeof(sbyte)) return "sbyte";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "long";
            if (type == typeof(ulong)) return "ulong";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(char)) return "char";
            if (type == typeof(string)) return "string";

            var sb = new StringBuilder();

            if (type.IsArray)
            {
                var rank = type.GetArrayRank();

                sb.Append(type.GetElementType().Describe());
                sb.Append($"[{new string(',',rank-1)}]");
            }
            else
            {
                sb.Append(type.Name.CleanMemberName());
            }

            if (type.IsGenericType || type.IsGenericTypeDefinition)
            {
                sb.Append("<");
                var args = type.GetGenericArguments();
                sb.Append(string.Join(",", args.Select(x => x.Describe())));
                sb.Append(">");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Removes any `x args from Generic type names
        /// </summary>
        /// <returns></returns>
        public static string CleanMemberName(this string name)
        {
            var genericArg = name.IndexOf("`", StringComparison.Ordinal);

            if (genericArg <= 0)
                return name;

            return name.Substring(0, genericArg);
        }

        public static string Describe(this MethodInfo method)
        {
            var methParams = method.GetParameters();

            var param = methParams.Length == 0
                ? ""
                : string.Join(", ", methParams.Select(x => x.Describe()));

            var genericArg = method.IsGenericMethod || method.IsGenericMethodDefinition
                ? $"<{string.Join(",",method.GetGenericArguments().Select(x => x.Describe()))}>"
                : null;


            return $"{method.ReturnType.Describe()} {method.Name.CleanMemberName()}{genericArg}({param})";
        }

        public static string Describe(this ConstructorInfo ctor)
        {
            var methParams = ctor.GetParameters();

            var param = methParams.Length == 0
                ? ""
                : string.Join(", ", methParams.Select(x => x.Describe()));

            return $"{ctor.DeclaringType.Describe()}({param})";
        }

        public static string Describe(this ParameterInfo param)
        {
            return $"{param.ParameterType.Describe()} {param.Name}";
        }

        internal static bool IsInjectable(this Type type, Predicate<Type> canInject)
        {
            return type.IsMockable()
                   || type.HasDefaultCtor()
                   || canInject(type);
        }


        internal static bool IsMockable(this Type type)
        {
            return type.IsInterface || type.IsAbstract;
        }

        internal static bool IsMockable(this MethodInfo method)
        {
            return method.IsVirtual ||
                   method.IsAbstract ||
                   method.IsVirtual;
        }

        public static bool IsPropertyGetter(this MethodInfo method)
        {
            return method.IsSpecialName && method.Name.StartsWith("get_", StringComparison.Ordinal);
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

        public static bool IsClosedGenericAssignableToOpenGenericType(this Type closedGivenType, Type openGenericType)
        {
            var interfaceTypes = closedGivenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == openGenericType)
                    return true;
            }

            if (closedGivenType.IsGenericType && closedGivenType.GetGenericTypeDefinition() == openGenericType)
                return true;

            Type baseType = closedGivenType.BaseType;
            if (baseType == null) return false;

            return IsClosedGenericAssignableToOpenGenericType(baseType, openGenericType);
        }

        public static bool IsOpenGenericAssignableToOpenGenericType(this Type openGenericFrom, Type openGenericTo)
        {
            if(!openGenericFrom.IsGenericType)
                throw new ArgumentOutOfRangeException(nameof(openGenericFrom), "Must be an Open Generic type, e.g. IList<>");

            if (!openGenericTo.IsGenericType)
                throw new ArgumentOutOfRangeException(nameof(openGenericTo), "Must be an Open Generic type, e.g. IList<>");

            if (openGenericFrom == openGenericTo)
                return true;

            var interfaces = openGenericFrom.GetInterfaces();

            foreach (var it in interfaces)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == openGenericTo)
                    return true;
            }
            
            var baseType = openGenericFrom.BaseType;

            if (baseType == null)
                return false;

            return IsClosedGenericAssignableToOpenGenericType(baseType, openGenericTo);
        }
    }
}
