using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Moq;
using MoqqerNamespace.Helpers;

namespace MoqqerNamespace
{
    public class Moqqer
    {
        internal static MethodInfo ObjectGenericMethod;
        internal readonly Dictionary<Type, Mock> Mocks = new Dictionary<Type, Mock>();
        internal readonly Dictionary<Type, object> Objects = new Dictionary<Type, object>();

        public Moqqer()
        {
            Objects.Add(typeof(string), string.Empty);
        }

        static Moqqer()
        {
            var moqType = typeof(Moqqer);
            
            ObjectGenericMethod = moqType.GetGenericMethod(nameof(Moqqer.GetInstance));
        }

        public T Create<T>() where T : class
        {
            var type = typeof(T);

            var ctor = type.FindConstructor(HasObjectOrDefault);

            var parameters = CreateParameters(ctor);

            var res = ctor.Invoke(parameters);

            return res as T;
        }

        internal bool HasObjectOrDefault(Type type)
        {
            if (Objects.ContainsKey(type))
                return true;

            if (Default(type) != null)
                return true;

            if (Objects.ContainsKey(type))
                return true;

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

        public object Object(Type type)
        {
            if (Objects.ContainsKey(type))
                return Objects[type];

            var def = Default(type);
            if (def != null) return def;

            if (type.IsMockable())
                throw new MoqqerException($"Type('{type.Name}')  is mockable. Use Create<T>() if you are looking to create an object with injection of mocks/objects. Or Use the Mock<T>() if you want to retrieve a mock of that type which was/will be injected.");
            
            var ctor = type.GetDefaultCtor();

            if (ctor == null)
                throw new MoqqerException($"Cannot get Type('{type.Name}') as it does not have a default constructor. If you meant to create an object with injection into the Ctor use Create<T>()");

            var res = ctor.Invoke(null);

            AddToObjects(type, res);

            return res;
        }

        private object AddToObjects(Type type, object res)
        {
            Objects.Add(type, res);
            return res;
        }

        private object Default(Type type)
        {
            if (type.IsGenericType)
                return DefaultGeneric(type);
            
            if (type == typeof(string))
                return AddToObjects(type, string.Empty);

            return null;
        }

        private object DefaultGeneric(Type type)
        {
            var generic = type.GetGenericTypeDefinition();

            if (generic == typeof(IList<>))
            {
                var listType = typeof(List<>).MakeGenericType(type.GetGenericArguments());
                var list = Object(listType);
                return AddToObjects(type, list);
            }

            return null;
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
            var parameters = ctor.GetParameters();

            var res = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var type = parameters[i].ParameterType;

                res[i] = GetParameter(type);
            }

            return res;
        }

        internal T GetInstance<T>()
        {
            return (T) GetInstance(typeof(T));
        }

        internal object GetInstance(Type type)
        {
            if (type.IsMockable())
                return Of(type).Object;

            return Object(type);
        }

        internal object GetParameter(Type type)
        {
            return GetInstance(type);
        }

        internal bool HasParameterlessCtor(Type type)
        {
            return null == TypeHelpers.GetDefaultCtor(type);
        }

        internal static Mock MockOfType(Type type)
        {
            var mock = typeof(Mock<>);

            var genericMock = mock.MakeGenericType(type);

            return Activator.CreateInstance(genericMock) as Mock;
        }

        internal Mock Of(Type type)
        {
            if (Mocks.ContainsKey(type))
                return Mocks[type];

            var mock = MockOfType(type);

            Mocks.Add(type, mock);

            SetupMockMethods(mock, type);

            return mock;
        }

        internal void SetupMockMethods(Mock mock, Type type)
        {
            var methods = type.GetMockableMethods().ToList();

            if (!methods.Any()) return;

            var mockType = typeof(Mock<>).MakeGenericType(type);

            var mockSetupFuncMethod = mockType.GetMethods()
                .Single(x => x.Name == "Setup" && x.ContainsGenericParameters);

            var itType = typeof(It);
            var isAnyMethod = itType.GetMethod("IsAny");

            var inputParameter = Expression.Parameter(type, "x"); // Moqqer Lambda Param

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var args =
                    parameters.Select(x => Expression.Call(isAnyMethod.MakeGenericMethod(x.ParameterType))).ToArray();

                var reflectedExpression = Expression.Call(inputParameter, method, args);

                var setupFuncType = typeof(Func<,>).MakeGenericType(type, method.ReturnType);

                var lambda = Expression.Lambda(setupFuncType, reflectedExpression, inputParameter);

                var genericMockSetupFuncMethod = mockSetupFuncMethod.MakeGenericMethod(method.ReturnType);
                var setup = genericMockSetupFuncMethod.Invoke(mock, new object[] {lambda});

                var funcType = typeof(Func<>).MakeGenericType(method.ReturnType);

                var delgate = Delegate.CreateDelegate(funcType, this,
                    ObjectGenericMethod.MakeGenericMethod(method.ReturnType));

                var setupType = setup.GetType();

                var returnsMethod = setupType.GetMethod("Returns", new[] {funcType});

                returnsMethod.Invoke(setup, new object[] {delgate});
            }
        }


    }
}