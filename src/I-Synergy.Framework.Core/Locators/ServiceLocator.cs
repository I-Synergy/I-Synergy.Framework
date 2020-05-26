﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Exceptions;

namespace ISynergy.Framework.Core.Locators
{
    /// <summary>
    /// This class provides the ambient container for this application. If your
    /// framework defines such an ambient container, use ServiceLocator.Current
    /// to get it.
    /// </summary>
    public class ServiceLocator : IServiceLocator
    {
        /// <summary>
        /// The constructor infos
        /// </summary>
        private readonly Dictionary<Type, ConstructorInfo> _constructorInfos
            = new Dictionary<Type, ConstructorInfo>();

        /// <summary>
        /// The default key
        /// </summary>
        private readonly string _defaultKey = Guid.NewGuid().ToString();

        /// <summary>
        /// The empty arguments
        /// </summary>
        private readonly object[] _emptyArguments = new object[0];

        /// <summary>
        /// The factories
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, Delegate>> _factories
            = new Dictionary<Type, Dictionary<string, Delegate>>();

        /// <summary>
        /// The instances registry
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, object>> _instancesRegistry
            = new Dictionary<Type, Dictionary<string, object>>();

        /// <summary>
        /// The interface to class map
        /// </summary>
        private readonly Dictionary<Type, Type> _interfaceToClassMap
            = new Dictionary<Type, Type>();

        /// <summary>
        /// The synchronize lock
        /// </summary>
        private readonly object _syncLock = new object();
        /// <summary>
        /// The instance lock
        /// </summary>
        private static readonly object _instanceLock = new object();

        /// <summary>
        /// The default
        /// </summary>
        private static ServiceLocator _default;

        /// <summary>
        /// This class' default instance.
        /// </summary>
        /// <value>The default.</value>
        public static ServiceLocator Default
        {
            get
            {
                if (_default == null)
                {
                    lock (_instanceLock)
                    {
                        if (_default == null)
                        {
                            _default = new ServiceLocator();
                        }
                    }
                }

                return _default;
            }
        }

        /// <summary>
        /// Checks whether at least one instance of a given class is already created in the container.
        /// </summary>
        /// <typeparam name="TClass">The class that is queried.</typeparam>
        /// <returns>True if at least on instance of the class is already created, false otherwise.</returns>
        public bool ContainsCreated<TClass>()
        {
            return ContainsCreated<TClass>(null);
        }

        /// <summary>
        /// Checks whether the instance with the given key is already created for a given class
        /// in the container.
        /// </summary>
        /// <typeparam name="TClass">The class that is queried.</typeparam>
        /// <param name="key">The key that is queried.</param>
        /// <returns>True if the instance with the given key is already registered for the given class,
        /// false otherwise.</returns>
        public bool ContainsCreated<TClass>(string key)
        {
            var classType = typeof(TClass);

            if (!_instancesRegistry.ContainsKey(classType))
            {
                return false;
            }

            if (string.IsNullOrEmpty(key))
            {
                return _instancesRegistry[classType].Count > 0;
            }

            return _instancesRegistry[classType].ContainsKey(key);
        }

        /// <summary>
        /// Gets a value indicating whether a given type T is already registered.
        /// </summary>
        /// <typeparam name="T">The type that the method checks for.</typeparam>
        /// <returns>True if the type is registered, false otherwise.</returns>
        public bool IsRegistered<T>()
        {
            var classType = typeof(T);
            return _interfaceToClassMap.ContainsKey(classType);
        }

        /// <summary>
        /// Gets a value indicating whether a given type T and a give key
        /// are already registered.
        /// </summary>
        /// <typeparam name="T">The type that the method checks for.</typeparam>
        /// <param name="key">The key that the method checks for.</param>
        /// <returns>True if the type and key are registered, false otherwise.</returns>
        public bool IsRegistered<T>(string key)
        {
            var classType = typeof(T);

            if (!_interfaceToClassMap.ContainsKey(classType)
                || !_factories.ContainsKey(classType))
            {
                return false;
            }

            return _factories[classType].ContainsKey(key);
        }

        /// <summary>
        /// Registers a given type for a given interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface for which instances will be resolved.</typeparam>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        public void Register<TInterface, TClass>()
            where TInterface : class
            where TClass : class, TInterface
        {
            Register<TInterface, TClass>(false);
        }

        /// <summary>
        /// Registers a given type for a given interface with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TInterface">The interface for which instances will be resolved.</typeparam>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Register<TInterface, TClass>(bool createInstanceImmediately)
            where TInterface : class
            where TClass : class, TInterface
        {
            lock (_syncLock)
            {
                var interfaceType = typeof(TInterface);
                var classType = typeof(TClass);

                if (_interfaceToClassMap.ContainsKey(interfaceType))
                {
                    if (_interfaceToClassMap[interfaceType] != classType)
                    {
#if DEBUG
                        // Avoid some issues in the designer when the ViewModelLocator is instantiated twice
                        if (!Helpers.DesignerLibrary.IsInDesignMode)
                        {
#endif
                            throw new InvalidOperationException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "There is already a class registered for {0}.",
                                    interfaceType.FullName));
#if DEBUG
                        }
#endif
                    }
                }
                else
                {
                    _interfaceToClassMap.Add(interfaceType, classType);
                    _constructorInfos.Add(classType, GetConstructorInfo(classType));
                }

                Func<TInterface> factory = MakeInstance<TInterface>;
                DoRegister(interfaceType, factory, _defaultKey);

                if (createInstanceImmediately)
                {
                    GetInstance<TInterface>();
                }
            }
        }

        /// <summary>
        /// Registers a given type.
        /// </summary>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        public void Register<TClass>()
            where TClass : class
        {
            Register<TClass>(false);
        }

        /// <summary>
        /// Registers a given type with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        /// <exception cref="ArgumentException">An interface cannot be registered alone.</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Register<TClass>(bool createInstanceImmediately)
            where TClass : class
        {
            var classType = typeof(TClass);
            if (classType.IsInterface)
            {
                throw new ArgumentException("An interface cannot be registered alone.");
            }

            lock (_syncLock)
            {
                if (_factories.ContainsKey(classType)
                    && _factories[classType].ContainsKey(_defaultKey))
                {
                    if (!_constructorInfos.ContainsKey(classType))
                    {
#if DEBUG
                        // Avoid some issues in the designer when the ViewModelLocator is instantiated twice
                        if (!Helpers.DesignerLibrary.IsInDesignMode)
                        {
#endif
                            // Throw only if constructor info have not been
                            // registered, which means there is a default factory
                            // for this class.
                            throw new InvalidOperationException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Class {0} is already registered.",
                                    classType));
#if DEBUG
                        }
#endif
                    }

                    return;
                }

                if (!_interfaceToClassMap.ContainsKey(classType))
                {
                    _interfaceToClassMap.Add(classType, null);
                }

                _constructorInfos.Add(classType, GetConstructorInfo(classType));
                Func<TClass> factory = MakeInstance<TClass>;
                DoRegister(classType, factory, _defaultKey);

                if (createInstanceImmediately)
                {
                    GetInstance<TClass>();
                }
            }
        }

        /// <summary>
        /// Registers a given instance for a given type.
        /// </summary>
        /// <typeparam name="TClass">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        public void Register<TClass>(Func<TClass> factory)
            where TClass : class
        {
            Register(factory, false);
        }

        /// <summary>
        /// Registers a given instance for a given type with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TClass">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        /// <exception cref="ArgumentNullException">factory</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Register<TClass>(Func<TClass> factory, bool createInstanceImmediately)
            where TClass : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            lock (_syncLock)
            {
                var classType = typeof(TClass);

                if (_factories.ContainsKey(classType)
                    && _factories[classType].ContainsKey(_defaultKey))
                {
#if DEBUG
                    // Avoid some issues in the designer when the ViewModelLocator is instantiated twice
                    if (!Helpers.DesignerLibrary.IsInDesignMode)
                    {
#endif
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "There is already a factory registered for {0}.",
                                classType.FullName));
#if DEBUG
                    }
#endif
                }

                if (!_interfaceToClassMap.ContainsKey(classType))
                {
                    _interfaceToClassMap.Add(classType, null);
                }

                DoRegister(classType, factory, _defaultKey);

                if (createInstanceImmediately)
                {
                    GetInstance<TClass>();
                }
            }
        }

        /// <summary>
        /// Registers a given instance for a given type and a given key.
        /// </summary>
        /// <typeparam name="TClass">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        /// <param name="key">The key for which the given instance is registered.</param>
        public void Register<TClass>(Func<TClass> factory, string key)
            where TClass : class
        {
            Register(factory, key, false);
        }

        /// <summary>
        /// Registers a given instance for a given type and a given key with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TClass">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        /// <param name="key">The key for which the given instance is registered.</param>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        /// <exception cref="ArgumentNullException">factory</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Register<TClass>(
            Func<TClass> factory,
            string key,
            bool createInstanceImmediately)
            where TClass : class
        {

            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            lock (_syncLock)
            {
                var classType = typeof(TClass);

                if (_factories.ContainsKey(classType)
                    && _factories[classType].ContainsKey(key))
                {
#if DEBUG
                    // Avoid some issues in the designer when the ViewModelLocator is instantiated twice
                    if (!Helpers.DesignerLibrary.IsInDesignMode)
                    {
#endif
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "There is already a factory registered for {0} with key {1}.",
                                classType.FullName,
                                key));
#if DEBUG
                    }
#endif
                }

                if (!_interfaceToClassMap.ContainsKey(classType))
                {
                    _interfaceToClassMap.Add(classType, null);
                }

                DoRegister(classType, factory, key);

                if (createInstanceImmediately)
                {
                    GetInstance<TClass>(key);
                }
            }
        }

        /// <summary>
        /// Resets the instance in its original states. This deletes all the
        /// registrations.
        /// </summary>
        public void Reset()
        {
            _interfaceToClassMap.Clear();
            _instancesRegistry.Clear();
            _constructorInfos.Clear();
            _factories.Clear();
        }

        /// <summary>
        /// Unregisters a class from the cache and removes all the previously
        /// created instances.
        /// </summary>
        /// <typeparam name="TClass">The class that must be removed.</typeparam>
        public void Unregister<TClass>()
            where TClass : class
        {
            lock (_syncLock)
            {
                var serviceType = typeof(TClass);
                Type resolveTo;

                if (_interfaceToClassMap.ContainsKey(serviceType))
                {
                    resolveTo = _interfaceToClassMap[serviceType] ?? serviceType;
                }
                else
                {
                    resolveTo = serviceType;
                }

                if (_instancesRegistry.ContainsKey(serviceType))
                {
                    _instancesRegistry.Remove(serviceType);
                }

                if (_interfaceToClassMap.ContainsKey(serviceType))
                {
                    _interfaceToClassMap.Remove(serviceType);
                }

                if (_factories.ContainsKey(serviceType))
                {
                    _factories.Remove(serviceType);
                }

                if (_constructorInfos.ContainsKey(resolveTo))
                {
                    _constructorInfos.Remove(resolveTo);
                }
            }
        }

        /// <summary>
        /// Removes the given instance from the cache. The class itself remains
        /// registered and can be used to create other instances.
        /// </summary>
        /// <typeparam name="TClass">The type of the instance to be removed.</typeparam>
        /// <param name="instance">The instance that must be removed.</param>
        public void Unregister<TClass>(TClass instance)
            where TClass : class
        {
            lock (_syncLock)
            {
                var classType = typeof(TClass);

                if (_instancesRegistry.ContainsKey(classType))
                {
                    var list = _instancesRegistry[classType];

                    var pairs = list.Where(pair => pair.Value == instance).ToList();
                    for (var index = 0; index < pairs.Count(); index++)
                    {
                        var key = pairs[index].Key;

                        list.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the instance corresponding to the given key from the cache. The class itself remains
        /// registered and can be used to create other instances.
        /// </summary>
        /// <typeparam name="TClass">The type of the instance to be removed.</typeparam>
        /// <param name="key">The key corresponding to the instance that must be removed.</param>
        public void Unregister<TClass>(string key)
            where TClass : class
        {
            lock (_syncLock)
            {
                var classType = typeof(TClass);

                if (_instancesRegistry.ContainsKey(classType))
                {
                    var list = _instancesRegistry[classType];

                    var pairs = list.Where(pair => pair.Key == key).ToList();
                    for (var index = 0; index < pairs.Count(); index++)
                    {
                        list.Remove(pairs[index].Key);
                    }
                }

                if (_factories.ContainsKey(classType))
                {
                    if (_factories[classType].ContainsKey(key))
                    {
                        _factories[classType].Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Does the get service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="key">The key.</param>
        /// <param name="cache">if set to <c>true</c> [cache].</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="ActivationException"></exception>
        /// <exception cref="ActivationException"></exception>
        private object DoGetService(Type serviceType, string key, bool cache = true)
        {
            lock (_syncLock)
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = _defaultKey;
                }

                Dictionary<string, object> instances = null;

                if (!_instancesRegistry.ContainsKey(serviceType))
                {
                    if (!_interfaceToClassMap.ContainsKey(serviceType))
                    {
                        throw new ActivationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Type not found in cache: {0}.",
                                serviceType.FullName));
                    }

                    if (cache)
                    {
                        instances = new Dictionary<string, object>();
                        _instancesRegistry.Add(serviceType, instances);
                    }
                }
                else
                {
                    instances = _instancesRegistry[serviceType];
                }

                if (instances != null
                    && instances.ContainsKey(key))
                {
                    return instances[key];
                }

                object instance = null;

                if (_factories.ContainsKey(serviceType))
                {
                    if (_factories[serviceType].ContainsKey(key))
                    {
                        instance = _factories[serviceType][key].DynamicInvoke(null);
                    }
                    else
                    {
                        if (_factories[serviceType].ContainsKey(_defaultKey))
                        {
                            instance = _factories[serviceType][_defaultKey].DynamicInvoke(null);
                        }
                        else
                        {
                            throw new ActivationException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Type not found in cache without a key: {0}",
                                    serviceType.FullName));
                        }
                    }
                }

                if (cache
                    && instances != null)
                {
                    instances.Add(key, instance);
                }

                return instance;
            }
        }

        /// <summary>
        /// Does the register.
        /// </summary>
        /// <typeparam name="TClass">The type of the t class.</typeparam>
        /// <param name="classType">Type of the class.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="key">The key.</param>
        private void DoRegister<TClass>(Type classType, Func<TClass> factory, string key)
        {
            if (_factories.ContainsKey(classType))
            {
                if (_factories[classType].ContainsKey(key))
                {
                    // The class is already registered, ignore and continue.
                    return;
                }

                _factories[classType].Add(key, factory);
            }
            else
            {
                var list = new Dictionary<string, Delegate>
                {
                    {
                        key,
                        factory
                    }
                };

                _factories.Add(classType, list);
            }
        }

        /// <summary>
        /// Gets the constructor information.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>ConstructorInfo.</returns>
        /// <exception cref="ActivationException"></exception>
        /// <exception cref="ActivationException"></exception>
        private ConstructorInfo GetConstructorInfo(Type serviceType)
        {
            Type resolveTo;

            if (_interfaceToClassMap.ContainsKey(serviceType))
            {
                resolveTo = _interfaceToClassMap[serviceType] ?? serviceType;
            }
            else
            {
                resolveTo = serviceType;
            }

            var constructorInfos = resolveTo.GetConstructors();

            if (constructorInfos.Length > 1)
            {
                if (constructorInfos.Length > 2)
                {
                    return GetPreferredConstructorInfo(constructorInfos, resolveTo);
                }

                if (constructorInfos.FirstOrDefault(i => i.Name == ".cctor") == null)
                {
                    return GetPreferredConstructorInfo(constructorInfos, resolveTo);
                }

                var first = constructorInfos.FirstOrDefault(i => i.Name != ".cctor");

                if (first == null
                    || !first.IsPublic)
                {
                    throw new ActivationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Cannot register: No public constructor found in {0}.",
                            resolveTo.Name));
                }

                return first;
            }

            if (constructorInfos.Length == 0
                || (constructorInfos.Length == 1
                    && !constructorInfos[0].IsPublic))
            {
                throw new ActivationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cannot register: No public constructor found in {0}.",
                        resolveTo.Name));
            }

            return constructorInfos[0];
        }

        /// <summary>
        /// Gets the preferred constructor information.
        /// </summary>
        /// <param name="constructorInfos">The constructor infos.</param>
        /// <param name="resolveTo">The resolve to.</param>
        /// <returns>ConstructorInfo.</returns>
        /// <exception cref="ActivationException"></exception>
        private static ConstructorInfo GetPreferredConstructorInfo(IEnumerable<ConstructorInfo> constructorInfos, Type resolveTo)
        {
            var preferredConstructorInfo
                = (from t in constructorInfos
                   let attribute = Attribute.GetCustomAttribute(t, typeof(PreferredConstructorAttribute))
                   where attribute != null
                   select t).FirstOrDefault();

            if (preferredConstructorInfo == null)
            {
                throw new ActivationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cannot register: Multiple constructors found in {0} but none marked with PreferredConstructor.",
                        resolveTo.Name));
            }

            return preferredConstructorInfo;
        }

        /// <summary>
        /// Makes the instance.
        /// </summary>
        /// <typeparam name="TClass">The type of the t class.</typeparam>
        /// <returns>TClass.</returns>
        private TClass MakeInstance<TClass>()
        {
            var serviceType = typeof(TClass);

            var constructor = _constructorInfos.ContainsKey(serviceType)
                                  ? _constructorInfos[serviceType]
                                  : GetConstructorInfo(serviceType);

            var parameterInfos = constructor.GetParameters();

            if (parameterInfos.Length == 0)
            {
                return (TClass)constructor.Invoke(_emptyArguments);
            }

            var parameters = new object[parameterInfos.Length];

            foreach (var parameterInfo in parameterInfos)
            {
                parameters[parameterInfo.Position] = GetService(parameterInfo.ParameterType);
            }

            return (TClass)constructor.Invoke(parameters);
        }

        /// <summary>
        /// Provides a way to get all the created instances of a given type available in the
        /// cache. Registering a class or a factory does not automatically
        /// create the corresponding instance! To create an instance, either register
        /// the class or the factory with createInstanceImmediately set to true,
        /// or call the GetInstance method before calling GetAllCreatedInstances.
        /// Alternatively, use the GetAllInstances method, which auto-creates default
        /// instances for all registered classes.
        /// </summary>
        /// <param name="serviceType">The class of which all instances
        /// must be returned.</param>
        /// <returns>All the already created instances of the given type.</returns>
        public IEnumerable<object> GetAllCreatedInstances(Type serviceType)
        {
            if (_instancesRegistry.ContainsKey(serviceType))
            {
                return _instancesRegistry[serviceType].Values;
            }

            return new List<object>();
        }

        /// <summary>
        /// Provides a way to get all the created instances of a given type available in the
        /// cache. Registering a class or a factory does not automatically
        /// create the corresponding instance! To create an instance, either register
        /// the class or the factory with createInstanceImmediately set to true,
        /// or call the GetInstance method before calling GetAllCreatedInstances.
        /// Alternatively, use the GetAllInstances method, which auto-creates default
        /// instances for all registered classes.
        /// </summary>
        /// <typeparam name="TService">The class of which all instances
        /// must be returned.</typeparam>
        /// <returns>All the already created instances of the given type.</returns>
        public IEnumerable<TService> GetAllCreatedInstances<TService>()
        {
            var serviceType = typeof(TService);
            return GetAllCreatedInstances(serviceType)
                .Select(instance => (TService)instance);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType" />.</returns>
        /// <exception cref="ActivationException">If the type serviceType has not
        /// been registered before calling this method.</exception>
        public object GetService(Type serviceType)
        {
            return DoGetService(serviceType, _defaultKey);
        }

        /// <summary>
        /// Provides a way to get all the created instances of a given type available in the
        /// cache. Calling this method auto-creates default
        /// instances for all registered classes.
        /// </summary>
        /// <param name="serviceType">The class of which all instances
        /// must be returned.</param>
        /// <returns>All the instances of the given type.</returns>
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            lock (_factories)
            {
                if (_factories.ContainsKey(serviceType))
                {
                    foreach (var factory in _factories[serviceType])
                    {
                        GetInstance(serviceType, factory.Key);
                    }
                }
            }

            if (_instancesRegistry.ContainsKey(serviceType))
            {
                return _instancesRegistry[serviceType].Values;
            }


            return new List<object>();
        }

        /// <summary>
        /// Provides a way to get all the created instances of a given type available in the
        /// cache. Calling this method auto-creates default
        /// instances for all registered classes.
        /// </summary>
        /// <typeparam name="TService">The class of which all instances
        /// must be returned.</typeparam>
        /// <returns>All the instances of the given type.</returns>
        public IEnumerable<TService> GetAllInstances<TService>()
        {
            var serviceType = typeof(TService);
            return GetAllInstances(serviceType)
                .Select(instance => (TService)instance);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. If no instance had been instantiated
        /// before, a new instance will be created. If an instance had already
        /// been created, that same instance will be returned.
        /// </summary>
        /// <param name="serviceType">The class of which an instance
        /// must be returned.</param>
        /// <returns>An instance of the given type.</returns>
        /// <exception cref="ActivationException">If the type serviceType has not
        /// been registered before calling this method.</exception>
        public object GetInstance(Type serviceType)
        {
            return DoGetService(serviceType, _defaultKey);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. This method
        /// always returns a new instance and doesn't cache it in the IOC container.
        /// </summary>
        /// <param name="serviceType">The class of which an instance
        /// must be returned.</param>
        /// <returns>An instance of the given type.</returns>
        /// <exception cref="ActivationException">If the type serviceType has not
        /// been registered before calling this method.</exception>
        public object GetInstanceWithoutCaching(Type serviceType)
        {
            return DoGetService(serviceType, _defaultKey, false);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type corresponding
        /// to a given key. If no instance had been instantiated with this
        /// key before, a new instance will be created. If an instance had already
        /// been created with the same key, that same instance will be returned.
        /// </summary>
        /// <param name="serviceType">The class of which an instance must be returned.</param>
        /// <param name="key">The key uniquely identifying this instance.</param>
        /// <returns>An instance corresponding to the given type and key.</returns>
        /// <exception cref="ActivationException">If the type serviceType has not
        /// been registered before calling this method.</exception>
        public object GetInstance(Type serviceType, string key)
        {
            return DoGetService(serviceType, key);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. This method
        /// always returns a new instance and doesn't cache it in the IOC container.
        /// </summary>
        /// <param name="serviceType">The class of which an instance must be returned.</param>
        /// <param name="key">The key uniquely identifying this instance.</param>
        /// <returns>An instance corresponding to the given type and key.</returns>
        /// <exception cref="ActivationException">If the type serviceType has not
        /// been registered before calling this method.</exception>
        public object GetInstanceWithoutCaching(Type serviceType, string key)
        {
            return DoGetService(serviceType, key, false);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. If no instance had been instantiated
        /// before, a new instance will be created. If an instance had already
        /// been created, that same instance will be returned.
        /// </summary>
        /// <typeparam name="TService">The class of which an instance
        /// must be returned.</typeparam>
        /// <returns>An instance of the given type.</returns>
        /// <exception cref="ActivationException">If the type TService has not
        /// been registered before calling this method.</exception>
        public TService GetInstance<TService>()
        {
            return (TService)DoGetService(typeof(TService), _defaultKey);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. This method
        /// always returns a new instance and doesn't cache it in the IOC container.
        /// </summary>
        /// <typeparam name="TService">The class of which an instance
        /// must be returned.</typeparam>
        /// <returns>An instance of the given type.</returns>
        /// <exception cref="ActivationException">If the type TService has not
        /// been registered before calling this method.</exception>
        public TService GetInstanceWithoutCaching<TService>()
        {
            return (TService)DoGetService(typeof(TService), _defaultKey, false);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type corresponding
        /// to a given key. If no instance had been instantiated with this
        /// key before, a new instance will be created. If an instance had already
        /// been created with the same key, that same instance will be returned.
        /// </summary>
        /// <typeparam name="TService">The class of which an instance must be returned.</typeparam>
        /// <param name="key">The key uniquely identifying this instance.</param>
        /// <returns>An instance corresponding to the given type and key.</returns>
        /// <exception cref="ActivationException">If the type TService has not
        /// been registered before calling this method.</exception>
        public TService GetInstance<TService>(string key)
        {
            return (TService)DoGetService(typeof(TService), key);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. This method
        /// always returns a new instance and doesn't cache it in the IOC container.
        /// </summary>
        /// <typeparam name="TService">The class of which an instance must be returned.</typeparam>
        /// <param name="key">The key uniquely identifying this instance.</param>
        /// <returns>An instance corresponding to the given type and key.</returns>
        /// <exception cref="ActivationException">If the type TService has not
        /// been registered before calling this method.</exception>
        public TService GetInstanceWithoutCaching<TService>(string key)
        {
            return (TService)DoGetService(typeof(TService), key, false);
        }
    }
}
