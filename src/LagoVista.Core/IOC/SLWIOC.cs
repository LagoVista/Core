using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.IOC
{
    public static class SLWIOC
    {
        private static IDictionary<Type, Object> _registeredInstances = new Dictionary<Type, Object>();

        private static IDictionary<Type, Type> _registeredTypes = new Dictionary<Type, Type>();

        public static void Register<I>(Object instance)
        {
            _registeredInstances.Add(typeof(I), instance);
        }

        public static void RegisterSingleton<TInterface, TType>() where TInterface : class where TType: class
        {            
            var newType = CreateForType<TType>();
            _registeredInstances.Add(typeof(TInterface), newType);
        }

        public static void RegisterSingleton<TInterface>(object instance)
        {
            _registeredInstances.Add(typeof(TInterface), instance);
        }

        public static ITSingleton Get<ITSingleton>() where ITSingleton : class
        {
            return _registeredInstances[typeof(ITSingleton)] as ITSingleton;
        }

        public static void Register<I>(Type instanceType)
        {
            _registeredTypes.Add(typeof(I), instanceType);
        }

        public static void Register<I,T>()
        {
            _registeredTypes.Add(typeof(I), typeof(T));
        }

        public static TNewInstance Create<TNewInstance>() where TNewInstance : class
        {
            var type = _registeredTypes[typeof(TNewInstance)];
            var constructors = type.GetTypeInfo().DeclaredConstructors;
            var primaryConstructor = constructors.FirstOrDefault();
            var parameters = GetDependencyInjectionParameters(type, primaryConstructor);
            object newObject;
            try
            {
                newObject = primaryConstructor.Invoke(parameters.ToArray());
            }
            catch(Exception ex)
            {
                throw new Exception($"Could not create instance of type {typeof(TNewInstance)}", ex);
            }

            return newObject as TNewInstance;
        }

        public static TClassType CreateForType<TClassType>() where TClassType : class
        {
            var newInstanceType = typeof(TClassType);
            var constructors = newInstanceType.GetTypeInfo().DeclaredConstructors;
            var primaryConstructor = constructors.FirstOrDefault();
            var parameters = GetDependencyInjectionParameters(newInstanceType, primaryConstructor);
            object newObject;
            try
            {
                newObject = primaryConstructor.Invoke(parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not create instance of type {typeof(TClassType)}", ex);
            }

            return newObject as TClassType;
        }

        public static object CreateForType(Type newInstanceType) 
        {
            var constructors = newInstanceType.GetTypeInfo().DeclaredConstructors;
            var primaryConstructor = constructors.FirstOrDefault();
            var parameters = GetDependencyInjectionParameters(newInstanceType, primaryConstructor);
            object newObject;
            try
            {
                newObject = primaryConstructor.Invoke(parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not create instance of type {newInstanceType.FullName}", ex);
            }

            return newObject;
        }


        public static bool TryResolve<T>(out T value) where T : class
        {
            if (_registeredInstances.Keys.Contains(typeof(T)))
            {
                value = _registeredInstances[typeof(T)] as T;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public static bool TryResolve(Type type, out object value)
        {
            if(_registeredInstances.Keys.Contains(type))
            {
                value = _registeredInstances[type];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        private static List<object> GetDependencyInjectionParameters(Type type, ConstructorInfo primaryConstructor)
        {
            var parameters = new List<object>();

            foreach (var parameterInfo in primaryConstructor.GetParameters())
            {
                object parameterValue;

                if (!TryResolve(parameterInfo.ParameterType, out parameterValue))
                {
                    if (parameterInfo.IsOptional)
                    {
                        parameterValue = Type.Missing;
                    }
                    else
                    {
                        throw new Exception($"Could not create dependency for {parameterInfo.ParameterType.FullName} on {type.FullName}");
                    }
                }

                parameters.Add(parameterValue);
            }

            return parameters;
        }

        public static bool Contains(Type type)
        {
            return _registeredTypes.ContainsKey(type);
        }
    }
}
