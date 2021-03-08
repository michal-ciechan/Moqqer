using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MoqqerNamespace.DefaultFactories;
using MoqqerNamespace.Helpers;
using MoqqerNamespace.MoqqerQueryable;

namespace MoqqerNamespace
{
    public class Moqqer
    {
        /// <summary>
        /// Should this throw when trying to select a value type from a reference type. 
        /// E.g. x => x.NavigationProperty.Integer
        /// Should throw an exception if SomeNullableType is Null? 
        /// Because returning default SomeValueType is incorrect
        /// Correct expression should be x => (int?) x.NavigationProperty.Integer 
        /// so that null can be returned when NavigationProperty is null
        /// </summary>
        public static bool ThrowOnNonNullableReferenceTypeSelection
        {
            get => MoqqerExpressionRewriter.ThrowOnNonNullableReferenceTypeSelection;
            set => MoqqerExpressionRewriter.ThrowOnNonNullableReferenceTypeSelection = value;
        }

        private static readonly MethodInfo GetInstanceGenericMethod;
        private static readonly MethodInfo GetInstanceFuncGenericMethod;
        private static readonly MethodInfo MoqItIsAnyGenericMethod;
        private static readonly MethodInfo DefaultArrayGenericMethod;
        internal readonly Dictionary<Type, Mock> Mocks = new Dictionary<Type, Mock>();
        internal readonly Dictionary<Type, object> Objects = new Dictionary<Type, object>();
        internal readonly Dictionary<Type, IFactory> Factories = new Dictionary<Type, IFactory>();

        public bool UseMoqqerEnumerableQuery { get; set; } = true;
        public bool MockConcreteReturnTypes { get; set; }

        public Moqqer()
        {
            Objects.Add(typeof(string), string.Empty);
            Objects.Add(typeof(Task), TaskHelper.CompletedTask);
        }

        public List<IDefaultGenericFactory> DefaultGenericFactories { get; set; } = new List<IDefaultGenericFactory>
        {
            new ListDefaultGenericFactory(),
            new ObservableCollectionDefaultGenericFactory(),
            new QueryableDefaultGenericFactory(),
            new TaskDefaultGenericFactory(),
        };

        static Moqqer()
        {
            var moqType = typeof(Moqqer);
            
            GetInstanceGenericMethod = moqType.GetGenericMethod(nameof(Moqqer.GetInstance));
            GetInstanceFuncGenericMethod = moqType.GetGenericMethod(nameof(Moqqer.GetInstanceFunc));
            MoqItIsAnyGenericMethod = typeof(It).GetMethod("IsAny");
            DefaultArrayGenericMethod = moqType.GetGenericMethod(nameof(Moqqer.DefaultArray));
        }


        public T Create<T>(bool autogenerate = false) where T : class
        {
            try
            {
                return Create(typeof(T), autogenerate) as T;
            }
            catch (Exception e)
            {
                throw new MoqqerException($"Exception happened while trying to 'Create<{typeof(T).Describe()}>()'. See Inner exception for more details", e);
            }
        }

        private object Create(Type type, bool autogenerate)
        {
            var canCreate = autogenerate
                ? (Predicate<Type>) CanCreate
                : HasObjectOrDefault;

            var ctor = type.FindConstructor(canCreate);

            var parameters = CreateParameters(ctor);

            var res = ctor.Invoke(parameters);

            return res;
        }

        internal bool HasObjectOrDefault(Type type)
        {
            if (Objects.ContainsKey(type))
                return true;

            if (Default(type) != null)
                return true;

            if (Factories.ContainsKey(type))
                return true;

            return false;
        }

        internal bool CanCreate(Type type)
        {
            if (HasObjectOrDefault(type))
                return true;

            if (!type.IsValueType && !typeof(Delegate).IsAssignableFrom(type))
            {
                Use(Create(type, true), type);
                return true;
            }

            return false;
        }

        [Obsolete("Use Create<T>(). Will be depreciated soon")]
        public T Get<T>() where T : class
        {
            return Create<T>();
        }

        [Obsolete("")]
        public Mock<T> Mock<T>() where T : class
        {
            return Of<T>();
        }

        public T Object<T>() where T : class
        {
            var type = typeof(T);

            return Object(type) as T;
        }

		/// <summary>
		/// Returns a registered Default for the type if exists, otherwise either calls a parameterless ctor or returns `default(type)`
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
	    public object ObjectOrDefault(Type type)
	    {
		    var def = Default(type);
		    if (def != null) return def;

		    var ctor = type.GetDefaultCtor();

		    if (ctor == null)
			    return type.GetDefaultInstance();

		    var res = ctor.Invoke(null);

		    AddToObjects(type, res);

		    return res;
	    }

		/// <summary>
		/// Returns a registered Default for the type if exists, otherwise either calls a parameterless ctor or throws an exception
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public object Object(Type type)
        {
            var def = Default(type);
            if (def != null) return def;

            if (type.IsMockable())
                throw new MoqqerException($"Type('{type.Describe()}')  is mockable. " +
                                          $"Use Create<T>() if you are looking to create an object with injection of mocks/objects. " +
                                          $"Or Use the Of<T>() if you want to retrieve a mock of that type which was/will be injected.");
            
            var ctor = type.GetDefaultCtor();

            if (ctor == null)
                throw new MoqqerException($"Cannot get Type('{type.Describe()}') as it does not have a default constructor. " +
                                          $"If you meant to create an object with injection into the Ctor use Create<T>(). " +
                                          $"Alternatively use 'Moqqer.MockConcreteReturnTypes = true;' " +
                                          $"(see https://github.com/michal-ciechan/Moqqer#mock-concrete-return-types for more info)");

            var res = ctor.Invoke(null);

            AddToObjects(type, res);

            return res;
        }

        private object AddToObjects(Type type, object res)
        {
            Objects[type] = res;
            return res;
        }

        private object Default(Type type)
        {
            if(Objects.ContainsKey(type))
                return Objects[type];

            if (type.IsGenericType)
                return DefaultGeneric(type);
            
            if (type == typeof(string))
                return AddToObjects(type, string.Empty);

            if (type.IsArray)
                return DefaultArray(type);

            return null;
        }

        private object DefaultArray(Type type)
        {
            if(!type.IsArray)
                throw new ArgumentOutOfRangeException(nameof(type), "Must be a type of array T[]");

            var elementType = type.GetElementType();

            var res = DefaultArrayGenericMethod.MakeGenericMethod(elementType).Invoke(this, null);

            return res;
        }

        private T[] DefaultArray<T>()
        {
            var list = (List<T>) DefaultGeneric(typeof(List<T>));

            if(list == null)
                throw new Exception($"Could not get a DefaultGeneric list for List<{typeof(T).Name}>");

            return list.ToArray();
        }

        private object DefaultGeneric(Type type)
        {
            if (Objects.ContainsKey(type))
                return Objects[type];

            var openType = type.GetGenericTypeDefinition();

            var genericArguments = type.GetGenericArguments();

            // Cannot return Open Generic List
            if (genericArguments.Any(x => x.IsGenericParameter))
                return null;

            foreach (var factory in DefaultGenericFactories)
            {
                if(!factory.CanHandle(this, type, openType, genericArguments))
                    continue;

                var obj = factory.Create(this, type, openType, genericArguments);

                return AddToObjects(type, obj);
            }

            return null;
        }

        internal object GetOrCreateObject(Type type)
        {

            if (Objects.TryGetValue(type, out object res))
                return res;

            res = Activator.CreateInstance(type);

            Objects.Add(type, res);

            return res;
        }

        public Mock<T> Of<T>() where T : class
        {
            var type = typeof(T);

            if (Mocks.ContainsKey(type))
                return Mocks[type] as Mock<T>;

            var mock = new Mock<T>();

            Mocks.Add(type, mock);

            SetupMockMethods(mock, type);

            return mock;
        }


        public void VerifyAll()
        {
            foreach (var mock in Mocks.Values)
            {
                mock.VerifyAll();
            }
        }

        internal object[] CreateParameters(ConstructorInfo ctor)
        {
            try
            {
                var parameters = ctor.GetParameters();

                var res = new object[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var type = parameters[i].ParameterType;

                    res[i] = GetParameter(type, ctor);
                }

                return res;
            }
            catch (Exception e)
            {
                throw new MoqqerException($"Exception happened while trying to Create Parameters for Constructor '{ctor.Describe()}'. See Inner exception for more details.", e);
            }
        }

        internal T GetInstance<T>()
        {
            return (T) GetInstance(typeof(T));
        }

        private object GetInstanceFunc(Type type)
        {
            if(!type.IsGenericType)
                throw new Exception("Cannot get an instnace of a Func<T> because paramter 'type' is ot of Func<T>");

            var genericArgs = type.GetGenericArguments();

            if(genericArgs.Length > 1)
                throw new Exception("type has too many generic arguments! Can only provide closed Func<T> as type param");

            var returnType = genericArgs[0];

            // TODO: Create func to call factory if exists for return type

            var getFuncMethod = GetInstanceFuncGenericMethod.MakeGenericMethod(returnType);

            return getFuncMethod.Invoke(this, null);
        }

        private object GetInstanceFunc<T>()
        {
            Func<T> func = GetInstance<T>;

            return func;
        }

	    internal object GetInstanceOrDefault(Type type)
	    {
		    var def = Default(type);

		    if (def != null) return def;

		    if (type.IsMockable())
			    return Of(type).Object;

		    return ObjectOrDefault(type);
	    }

		internal object GetInstance(Type type)
        {
            var def = Default(type);
            
            if (def != null) return def;

            def = type.IsMockable() 
                ? Of(type).Object 
                : Object(type);                      

            if (Factories.TryGetValue(type, out var factory))
            {
                var res = factory.GetMethodParameter(type, null, null, def);

                return res;
            }

            return def;
        }
	    
	    

		internal bool CanGetDefaultOrMock(Type type)
        {
            return HasObjectOrDefault(type) || type.IsMockable();
        }

        internal object GetParameter(Type type, ConstructorInfo ctor)
		{
            if (Objects.TryGetValue(type, out var obj))
            {
                return obj;
            }
            
            var isFunc = type.IsFunc();

			if (Factories.TryGetValue(type, out IFactory factory))
			{
				var defaultParam = isFunc
					? GetInstanceFunc(type)
					: GetInstanceOrDefault(type);

				return factory.GetConstructorParameter(type, ctor, defaultParam);
			}

			var mocked = isFunc
				? GetInstanceFunc(type)
				: GetInstance(type);

			return mocked;
        }

        internal bool HasParameterlessCtor(Type type)
        {
            return null == type.GetDefaultCtor();
        }

        internal static Mock MockOfType(Type type)
        {
            var mock = typeof(Mock<>);

            var genericMock = mock.MakeGenericType(type);

            return Activator.CreateInstance(genericMock) as Mock;
        }

        internal Mock Of(Type type)
        {
            try
            {
                if (Mocks.ContainsKey(type))
                    return Mocks[type];

                var mock = MockOfType(type);

                Mocks.Add(type, mock);

                SetupMockMethods(mock, type);

                return mock;
            }
            catch (Exception e)
            {
                throw new MoqqerException($"Exception happened while trying to create a mock of '{type.Describe()}'. See inner exception for more details", e);
            }
        }

        public List<T> List<T>()
        {
            return DefaultGeneric(typeof(List<T>)) as List<T>;
        }

        internal void SetupMockMethods(Mock mock, Type type)
        {
            try
            {
                var canCreate = MockConcreteReturnTypes
                    ? (Predicate<Type>) CanCreate
                    : HasObjectOrDefault;

                var methods = type.GetMockableMethods(canCreate).ToList();

                if (!methods.Any()) return;

                var mockType = typeof(Mock<>).MakeGenericType(type);

                var mockSetupFuncMethod = mockType.GetMethods()
                    .Single(x => x.Name == "Setup" && x.ContainsGenericParameters);

                var inputParameter = Expression.Parameter(type, "x"); // Moqqer Lambda Param

                foreach (var method in methods)
                {
                    try
                    {
                        SetupMockMethod(mock, type, method, inputParameter, mockSetupFuncMethod);
                    }
                    catch (Exception e)
                    {
                        throw new MoqqerException($"Exception thrown while trying to setup Mock Method '{method.Describe()} on {type.Describe()}'", e);
                    }
                }
            }
            catch (Exception e)
            {
                throw new MoqqerException($"Exception thrown while trying to setup mock methods on '{type.Describe()}'. See inner exception for more details", e);
            }
        }

        private void SetupMockMethod(Mock mock, Type type, MethodInfo method, ParameterExpression inputParameter,
            MethodInfo mockSetupFuncMethod)
        {
            if (method.IsGenericMethod)
                return;

            if (!method.IsMockable())
                return;

            var parameters = method.GetParameters();

            // Issue #40 - Disable Mocking method with out parameter until 
            if (parameters.Any(x => x.IsOut))
                return;

            if (parameters.Any(x => x.ParameterType.IsByRef))
                return;
            
            var args =
                parameters.Select(
                        x => (Expression) Expression.Call(
                            MoqItIsAnyGenericMethod.MakeGenericMethod(x.ParameterType)))
                    .ToArray();

            var reflectedExpression = Expression.Call(inputParameter, method, args);

            var setupFuncType = typeof(Func<,>).MakeGenericType(type, method.ReturnType);

            var lambda = Expression.Lambda(setupFuncType, reflectedExpression, inputParameter);

            var genericMockSetupFuncMethod = mockSetupFuncMethod.MakeGenericMethod(method.ReturnType);
            var setup = genericMockSetupFuncMethod.Invoke(mock, new object[] {lambda});

            var funcType = typeof(Func<>).MakeGenericType(method.ReturnType);

            var setupType = setup.GetType();

            var instanceFunc = GetInstanceFunc(funcType, method);

            if (HasFactoryFor(method.ReturnType))
            {
                // Moq.Returns<TArg1,TArg2,...,T>(Func<TArg1, TArg2..., T> func)
                var returnsMethod = GetMoqReturnsMethod(method, setupType);

                // Func<TArg1, TArg2..., T>
                var returnDelegate = GetInstanceFactoryFunc(method, instanceFunc);

                returnsMethod.Invoke(setup, new[] {returnDelegate});
            }
            else
            {
                var returnsMethod = setupType.GetMethod("Returns", new[] {funcType});

                returnsMethod.Invoke(setup, new object[] {instanceFunc});
            }
        }

        private static MethodInfo GetMoqReturnsMethod(MethodInfo method, Type setupType)
        {
            var length = method.GetParameters().Length;

            if (length == 0)
            {
                var funcType = typeof(Func<>).MakeGenericType(method.ReturnType);

                return setupType.GetMethod("Returns", new[] { funcType });
            }

            var returnsMethodOpenGeneric = setupType.GetMethods()
                .FirstOrDefault(x => x.IsGenericMethod &&
                                     x.Name == "Returns" &&
                                     x.GetGenericArguments().Length == length);

            if(returnsMethodOpenGeneric == null)
                throw new MoqqerException($"Could not find Moq.Returns method for {setupType.Describe()} containing {length} generic arguments");

            var typeArguments = method.GetParameters().Select(x => x.ParameterType).ToArray();

            var returnsMethodClosed = returnsMethodOpenGeneric.MakeGenericMethod(typeArguments);

            return returnsMethodClosed;
        }

        private Delegate GetInstanceFunc(Type funcType, MethodInfo method)
        {
            var instanceFunc = Delegate.CreateDelegate(funcType, this,
                GetInstanceGenericMethod.MakeGenericMethod(method.ReturnType));

            return instanceFunc;
        }

        public bool HasFactoryFor(Type type)
        {
            return Factories.ContainsKey(type);
        }

        internal object GetInstanceFactoryFunc(MethodInfo method, Delegate instanceFunc)
        {
            if (!Factories.TryGetValue(method.ReturnType, out IFactory factory))
                throw new MoqqerException(
                    $"Could not get factory for creating instance for return type: '{method.ReturnType.Describe()}' for method '{method.Describe()}' on type '{method.DeclaringType.Describe()}'");
            
            // Factory Constant (for calling object GetMethodParameter(Type type, MethodInfo method, object[] args, object defaultMock);
            var factoryExpr = Expression.Constant(factory);
            var factoryMethod = factory.GetType().GetMethod(nameof(factory.GetMethodParameter));

            // Declaring Type
            var typeExpression = Expression.Constant(method.DeclaringType);

            // Method
            var calledMethodExpr = Expression.Constant(method);

            // Arguments
            var methodParams = method.GetParameters();

            var methodParamExpressions = methodParams
                .Select(x => Expression.Parameter(x.ParameterType))
                .ToArray();

            var methodParamConvertedExpressions = methodParamExpressions
                .Select(x => (Expression)Expression.Convert(x, typeof(object)))
                .ToArray();

            var arrayExpr = Expression.NewArrayInit(typeof(object), methodParamConvertedExpressions);

            // Default Mock
            var defaultMockExpression = Expression.Call(Expression.Constant(instanceFunc.Target), instanceFunc.Method);

            // Factory Call
            var getMethodParameterExpression = Expression.Call(factoryExpr, factoryMethod, typeExpression, calledMethodExpr, arrayExpr, defaultMockExpression);

            var getMethodParamConvertedExpression = Expression.Convert(getMethodParameterExpression, method.ReturnType);

            // Return method type
            var delegateType = GetMethodCallFunc(method);


            // Create Lambda
            var lambda = Expression.Lambda(delegateType, getMethodParamConvertedExpression,
                methodParamExpressions);

            var compiled = lambda.Compile();

            return compiled;
        }

        private Type GetMethodCallFunc(MethodInfo method)
        {
            var args = method.GetParameters();

            var funcType = GetFuncWithArgCount(args.Length);

            var genericArgs = args
                .Select(x => x.ParameterType)
                .Union(new[] {method.ReturnType})
                .ToArray();

            var delegateType = funcType.MakeGenericType(genericArgs);
            return delegateType;
        }

        private static Type GetFuncWithArgCount(int genericInputParamLength)
        {
            switch (genericInputParamLength)
            {
                case 0: return typeof(Func<>);
                case 1: return typeof(Func<,>);
                case 2: return typeof(Func<,,>);
                case 3: return typeof(Func<,,,>);
                case 4: return typeof(Func<,,,,>);
                case 5: return typeof(Func<,,,,,>);
                case 6: return typeof(Func<,,,,,,>);
                case 7: return typeof(Func<,,,,,,,>);
                case 8: return typeof(Func<,,,,,,,,>);
                case 9: return typeof(Func<,,,,,,,,,>);
                case 10: return typeof(Func<,,,,,,,,,,>);
                case 11: return typeof(Func<,,,,,,,,,,,>);
                case 12: return typeof(Func<,,,,,,,,,,,,>);
                case 13: return typeof(Func<,,,,,,,,,,,,,>);
                case 14: return typeof(Func<,,,,,,,,,,,,,,>);
                case 15: return typeof(Func<,,,,,,,,,,,,,,,>);
                case 16: return typeof(Func<,,,,,,,,,,,,,,,,>);
                default: throw new MoqqerException("Unsopported number of arguments in method");
            }

        }

        public IMoqqerObjectContext Use<T>(T implementation)
        {
            return Use(implementation, typeof(T));
        }

        public T Use<T>() where T : class
        {
            var obj = Create<T>();
            
            Use(obj, typeof(T)).ForAllImplementedInterfaces();

            return obj;
        }

        private IMoqqerObjectContext Use(object implementation, Type type)
        {
            Objects[type] = implementation;
            return new MoqqerObjectContext(this, implementation, type);
        }

        public void Factory<T>(Func<CallContext<T>, T> factoryFunction)
        {
            var factory = new Factory<T>(factoryFunction);

            Factories[typeof(T)] = factory;
        }
    }
}