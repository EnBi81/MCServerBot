using Microsoft.Extensions.DependencyInjection;
using Shared.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace MCWebAPI.Utils
{
    /// <summary>
    /// Extension methods for the Web Api
    /// </summary>
    public static class Extension
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
            collection.AddSingleton<TService, TImplementation>();
            CreateFactoryIfNotExist<TService, TImplementation>(collection, ServiceLifetime.Singleton, initAction);
            return collection;
        }


        
        private static void CreateFactoryIfNotExist<TService, TImplementation>(IServiceCollection serviceCollection, ServiceLifetime? serviceLifetime, Func<TImplementation, Task> initAction) where TService : class
        {
            var serviceType = typeof(TService);

            // get the service descriptor from which we need the instance
            var service = serviceCollection
                          .FirstOrDefault(s => (s.ServiceType == serviceType) &&
                            (serviceLifetime == null || s.Lifetime == serviceLifetime));

            if(service == null)
                throw new MCInternalException("Couldnt find service " + serviceType.FullName);

            var implementationType = service.ImplementationType ?? service.ServiceType;

            Func<IServiceProvider, TService> factory = prov => CreateInstance<TService, TImplementation>(prov, initAction);

            serviceCollection.Remove(service);
            serviceCollection.AddSingleton(serviceType, factory);
        }
        
        private static TService CreateInstance<TService, TImplementation>(IServiceProvider provider, Func<TImplementation, Task> initAction)
        {
            var serviceType = typeof(TService);
            var implementationType = typeof(TImplementation);

            // get the public constructor of the service which has parameters that are registered in the service collection
            var constructor = implementationType.GetConstructors()
                // select all the constructors which are public
                .Where(constructor => constructor.IsPublic)
                // select all the constructor with parameters which are in the service collection
                .Where(constructor => !constructor.GetParameters().Any(param => provider.GetService(param.ParameterType) is null))
                // get the constructor which has the most parameters
                .OrderByDescending(constructor => constructor.GetParameters().Length)
                // first constructor
                .FirstOrDefault();

            // throw if no public constructor available
            if (constructor == null)
                throw new MCInternalException("No available public constructor for " + serviceType.FullName);


            // get the constructor parameters together
            List<object> parameters = new();
            foreach (var parameter in constructor.GetParameters())
            {
                var createdParam = provider.GetService(parameter.ParameterType)!;
                parameters.Add(createdParam);
            }

            // call the constructor
            object? serviceInstance = Activator.CreateInstance(implementationType, parameters.ToArray());

            // if instance creation failed, throw
            if (serviceInstance == null)
                throw new MCInternalException("Failed to create object " + serviceType.FullName);

            initAction((TImplementation)serviceInstance).GetAwaiter().GetResult();

            // return the instance
            return (TService)serviceInstance;
        }
    }
}
