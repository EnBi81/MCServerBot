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

            TImplementation serviceInstance = (TImplementation)CreateInstanceIfNotExist(collection, typeof(TService), ServiceLifetime.Singleton);


            Task.Run(async () => await initAction(serviceInstance));

            return collection;
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
            // get the service descriptor from which we need the instance
            var service = serviceCollection
                .Where(s => (s.ServiceType == serviceType) && 
                            (serviceLifetime == null ? true : s.Lifetime == serviceLifetime))
                .FirstOrDefault();


            // check if the descriptor exists
            if (service == null)
                throw new Exception($"Service {serviceType.FullName} with lifetime {serviceLifetime} could not be found");

            // if the implementation already exists, then return it
            if (service.ImplementationInstance is not null)
                return service.ImplementationInstance;

            
            var implementationType = service.ImplementationType ?? service.ServiceType;

            var availableTypes = serviceCollection.Select(s => s.ServiceType);

            // get the public constructor of the service which has parameters that are registered in the service collection
            var constructor = implementationType.GetConstructors()
                // select all the constructors which are public
                .Where(constructor => constructor.IsPublic)
                // select all the constructor with parameters which are in the service collection
                .Where(constructor => !constructor.GetParameters().Where(param => !availableTypes.Contains(param.ParameterType)).Any())
                // get the constructor which has the most parameters
                .OrderByDescending(constructor => constructor.GetParameters().Length)
                // first constructor
                .FirstOrDefault();

            // throw if no public constructor available
            if (constructor == null)
                throw new Exception("No available public constructor for " + serviceType.FullName);


            // get the constructor parameters together
            List<object> parameters = new ();
            foreach(var parameter in constructor.GetParameters())
            {
                var createdParam = CreateInstanceIfNotExist(serviceCollection, parameter.ParameterType, null);
                parameters.Add(createdParam);
            }

            // call the constructor
            object? serviceInstance = Activator.CreateInstance(implementationType, parameters.ToArray());

            // if instance creation failed, throw
            if(serviceInstance == null)
                throw new Exception("Failed to create object " + serviceType.FullName);

            // remove the old descriptor
            serviceCollection.Remove(service);
            // add the new one
            serviceCollection.AddSingleton(serviceType, serviceInstance);


            // return the instance
            return serviceInstance;
        }
    }
}
