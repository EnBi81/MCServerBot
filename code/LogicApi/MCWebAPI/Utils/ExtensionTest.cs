using Microsoft.Extensions.DependencyInjection;
using Shared.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace MCWebAPI.Utils.Test
{
    /// <summary>
    /// Extension methods for the Web Api
    /// </summary>
    public static class ExtensionTest
    {
        /// <summary>
        /// Adds the Singleton service to the IServiceCollection, and runs the initAction afterwards.
        /// </summary>
        /// <typeparam name="TService">type of the service.</typeparam>
        /// <typeparam name="TImplementation">implementation type of the service.</typeparam>
        /// <param name="collection">the service collection</param>
        /// <param name="initAction">Init action. This will run right after the service is created.</param>
        /// <returns>The IServiceCollection instance with the Initialized service included.</returns>
        public static IServiceCollection AddSingletonAndInit<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>
            (this IServiceCollection collection, Func<TImplementation, Task> initAction)
            where TService : class
            where TImplementation : class, TService
        {
            return AddSingletonAndInit<TService, TImplementation>(collection, initAction, true);
        }

        /// <summary>
        /// Adds the Singleton service to the IServiceCollection, and runs the initAction afterwards.
        /// </summary>
        /// <typeparam name="TService">type of the service.</typeparam>
        /// <typeparam name="TImplementation">implementation type of the service.</typeparam>
        /// <param name="collection">the service collection</param>
        /// <param name="initAction">Init action. This will run right after the service is created.</param>
        /// <returns>The IServiceCollection instance with the Initialized service included.</returns>
        public static IServiceCollection AddSingletonAndInit<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>
            (this IServiceCollection collection, Func<TImplementation, Task> initAction, bool lazyInit)
            where TService : class
            where TImplementation : class, TService
        {
            collection.AddSingleton<TService, TImplementation>();

            if (lazyInit)
            {
                CreateFactoryIfNotExist<TService, TImplementation>(collection, ServiceLifetime.Singleton, initAction);
            }
            else
            {
                TImplementation serviceInstance = (TImplementation)CreateInstanceIfNotExist(collection, typeof(TService), ServiceLifetime.Singleton);
                Task.Run(async () => await initAction(serviceInstance));
            }

            return collection;
        }

        private static void GetService(IServiceCollection collection, Type serviceType, ServiceLifetime? lifetime, out ServiceDescriptor serviceDescriptor, out Type implementationType)
        {
            var service = collection
                          .FirstOrDefault(s => (s.ServiceType == serviceType) &&
                            (lifetime == null || s.Lifetime == lifetime));


            // check if the descriptor exists
            if (service == null)
                throw new MCInternalException($"Service {serviceType.FullName} with lifetime {lifetime} could not be found");

            serviceDescriptor = service;
            implementationType = service.ImplementationType ?? service.ServiceType;
        }

        /// <summary>
        /// Checks if the type of service already has an instance available, if not, then calls the constructor with the required parameters. 
        /// Registers the service instance to the service collection, and returns the service instance.
        /// </summary>
        /// <param name="serviceCollection">service collection</param>
        /// <param name="serviceType">type of the service to optionally create and then return</param>
        /// <param name="serviceLifetime">lifetime of the service</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static object CreateInstanceIfNotExist(IServiceCollection serviceCollection, Type serviceType, ServiceLifetime? serviceLifetime)
        {
            GetService(serviceCollection, serviceType, serviceLifetime, out var serviceDescriptor, out var implementationType);

            Func<Type, object?> getService = type =>
            {
                try
                {
                    return CreateInstanceIfNotExist(serviceCollection, type, null);
                }
                catch
                {
                    return null;
                }
            };

            
            var serviceInstance = CreateInstance(getService, implementationType);
            
            // remove the old descriptor
            serviceCollection.Remove(serviceDescriptor);
            // add the new one
            serviceCollection.AddSingleton(serviceType, serviceInstance);

            // return the instance
            return serviceInstance;
        }



        private static void CreateFactoryIfNotExist<TService, TImplementation>(IServiceCollection serviceCollection, ServiceLifetime? serviceLifetime, Func<TImplementation, Task> initAction) where TService : class
        {
            var serviceType = typeof(TService);

            GetService(serviceCollection, serviceType, serviceLifetime, out var serviceDescriptor, out var implementationType);

            Func<IServiceProvider, TService> factory = prov => 
            {
                var serviceInstance = CreateInstance(type => prov.GetService(type), typeof(TImplementation));
                initAction((TImplementation)serviceInstance).GetAwaiter().GetResult();
                return (TService)serviceInstance;
            };

            serviceCollection.Remove(serviceDescriptor);
            serviceCollection.AddSingleton(serviceType, factory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getService"></param>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        /// <exception cref="MCInternalException"></exception>
        private static object CreateInstance(Func<Type, object?> getService, Type implementationType)
        {
            // get the public constructor of the service which has parameters that are registered in the service collection
            var constructor = implementationType.GetConstructors()
                // select all the constructors which are public
                .Where(constructor => constructor.IsPublic)
                // select all the constructor with parameters which are in the service collection
                .Where(constructor => !constructor.GetParameters().Any(param => getService(param.ParameterType) is null))
                // get the constructor which has the most parameters
                .OrderByDescending(constructor => constructor.GetParameters().Length)
                // first constructor
                .FirstOrDefault();

            // throw if no public constructor available
            if (constructor == null)
                throw new MCInternalException("No available public constructor for " + implementationType.FullName);


            // get the constructor parameters together
            List<object> parameters = new();
            foreach (var parameter in constructor.GetParameters())
            {
                var createdParam = getService(parameter.ParameterType)!;
                parameters.Add(createdParam);
            }

            // call the constructor
            object? serviceInstance = Activator.CreateInstance(implementationType, parameters.ToArray());

            // if instance creation failed, throw
            if (serviceInstance == null)
                throw new MCInternalException("Failed to create object " + implementationType.FullName);
            

            // return the instance
            return serviceInstance;
        }
    }
}
