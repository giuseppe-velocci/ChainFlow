using ChainFlow.ChainFlows;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace ChainFlow.DependencyInjection
{
    public static class ChainBuilderExtension
    {
        /// <summary>
        /// Add a single IChainFlow registration
        /// </summary>
        /// <typeparam name="T">Type T must implement IChainFlow</typeparam>
        /// <param name="services">Current IServiceCollection</param>
        /// <param name="serviceLifetime">Lifetime for the registered IChainFlow</param>
        /// <returns>Current IServiceCollection</returns>
        public static IServiceCollection AddChainFlow<T>(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient) where T : class, IChainFlow
        {
            RegisterWithLifetime<T>(services, serviceLifetime);

            services.AddSingleton(sp => new ChainFlowRegistration(typeof(T), () => sp.GetRequiredService<T>()));

            return services;
        }

        /// <summary>
        /// Add an IBooleanRouterChainFlow registration
        /// </summary>
        /// <typeparam name="TRouterDispatcher">Concrete type of class implementing IRouterDispatcher<bool> with router logic</typeparam>
        /// <param name="services">Current IServiceCollection</param>
        /// <param name="serviceLifetime">Lifetime for the registered IChainFlow</param>
        /// <returns></returns>
        public static IServiceCollection AddBooleanRouterChainFlow<TRouterDispatcher>(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient) where TRouterDispatcher : class, IRouterDispatcher<bool>
        {
            RegisterWithLifetime<TRouterDispatcher>(services, serviceLifetime);
            services.AddSingleton(sp => new ChainFlowRegistration(
                typeof(IBooleanRouterFlow<TRouterDispatcher>), 
                () => new BooleanRouterFlow<TRouterDispatcher>(sp.GetRequiredService<TRouterDispatcher>())));

            return services;
        }

        private static void RegisterWithLifetime<T>(IServiceCollection services, ServiceLifetime serviceLifetime) where T : class
        {
            if (serviceLifetime == ServiceLifetime.Transient)
            {
                services.AddTransient<T>();
            }
            else if (serviceLifetime == ServiceLifetime.Scoped)
            {
                services.AddScoped<T>();
            }
            else
            {
                services.AddSingleton<T>();
            }
        }
    }
}
