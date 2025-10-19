// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6d933583bfbc23a94feed6b6635aa6ad2c90b36c501cecd1f3d7dcc7d7c6c0f5
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.WriteLine($"Added singlton instance for interface {typeof(TInterface).Name} of type {typeof(TType).Name}");
        }

        public static void RegisterSingleton<TInterface>(object instance)
        {
            Debug.WriteLine($"Added singleton instance for interface {typeof(TInterface).Name} of type {instance.GetType().Name}");
            _registeredInstances.Add(typeof(TInterface), instance);
        }

        public static ITSingleton Get<ITSingleton>() where ITSingleton : class
        {
            if(!_registeredInstances.ContainsKey(typeof(ITSingleton)))
            {
                Debug.WriteLine($"SLWIOC => Could not find Singleton Implementation for Type {typeof(ITSingleton).Name}");
                throw new Exception($"SLWIOC => Could not find Singleton Implementation for Type {typeof(ITSingleton).Name}");
            }

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
            if (!_registeredTypes.ContainsKey(typeof(TNewInstance)))
            {
                throw new Exception($"Could not find registered type for: {typeof(TNewInstance).Name} ");
            }

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
                throw new Exception($"Could not create instance of type {typeof(TNewInstance)} - {ex.Message}", ex);
            }

            return newObject as TNewInstance;
        }

        public static TClassType CreateForType<TClassType>() where TClassType : class
        {
            var newInstanceType = typeof(TClassType);
            var constructors = newInstanceType.GetTypeInfo().DeclaredConstructors;
            var primaryConstructor = constructors.FirstOrDefault();
            if(primaryConstructor == null)
            {
                throw new Exception($"Could not find first constructor for {typeof(TClassType)}");
            }

            var parameters = GetDependencyInjectionParameters(newInstanceType, primaryConstructor);
            object newObject;
            try
            {
                newObject = primaryConstructor.Invoke(parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not create instance of type {typeof(TClassType)} - {ex.Message}", ex);
            }

            return newObject as TClassType;
        }

        public static object CreateForType(Type newInstanceType) 
        {
            if (newInstanceType.GetTypeInfo().IsInterface)
            {
                if (!_registeredTypes.ContainsKey(newInstanceType))
                {
                    throw new Exception($"Could not create new instance for type {newInstanceType.Name}, registered type does not include this instance type.");
                }

                newInstanceType = _registeredTypes[newInstanceType];
            }


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
                throw new Exception($"Could not create instance of type {newInstanceType.FullName} - {ex.Message}", ex);
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
                Debug.WriteLine($"Attempt to resolve: {parameterInfo.ParameterType.Name}");
                
                /* First try to get it from a singleton */
                if (!TryResolve(parameterInfo.ParameterType, out object parameterValue))
                {
                    parameterValue = CreateForType(parameterInfo.ParameterType);
                    if (parameterValue == null)
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
                    else
                    {
                        parameters.Add(parameterValue);
                    }
                }
                else
                {
                    parameters.Add(parameterValue);
                }
            }

            return parameters;
        }

        public static void Unregister<T>()
        {
            SLWIOC.Unregister(typeof(T));
        }

        public static void Unregister(Type type)
        {
            if(_registeredTypes.ContainsKey(type))
            {
                _registeredTypes.Remove(type);
            }
        }

        public static bool Contains(Type type)
        {
            return _registeredTypes.ContainsKey(type);
        }
    }
}
