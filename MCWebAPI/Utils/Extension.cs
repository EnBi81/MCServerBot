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

            var service = (TImplementation)collection
                .Where(s => s.ServiceType == typeof(TService))
                .Select(s => s.ImplementationInstance)
                .First()!;

            Task.Run(async () => await initAction(service));

            return collection;
        }
    }
}
