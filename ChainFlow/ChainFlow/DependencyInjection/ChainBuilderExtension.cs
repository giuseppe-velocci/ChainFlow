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
        /// Add a single IChainFlow registration from factory with transient lifetime
        /// </summary>
        /// <typeparam name="T">Type T must implement IChainFlow</typeparam>
        /// <param name="services">Current IServiceCollection</param>
        /// <param name="factory">Factory method to build IChainFlow instance</param>
        /// <param name="flowTag">Suffix used to identify a single IChainFlow instance among many of the same type</param>
        /// <returns>Current IServiceCollection</returns>
        public static IServiceCollection AddChainFlow<T>(
            this IServiceCollection services,
            Func<IServiceProvider, T> factory,
            string flowTag) where T : class, IChainFlow
        {
            services.AddSingleton(sp => new ChainFlowRegistration(typeof(T), () => factory(sp), flowTag));
            return services;
        }

        /// <summary>
        /// Add an IBooleanRouterChainFlow registration
        /// </summary>
        /// <typeparam name="TRouterDispatcher">Concrete type of class implementing IRouterDispatcher<bool> with router logic</typeparam>
        /// <param name="services">Current IServiceCollection</param>
        /// <param name="dispatcherLifetime">Lifetime for the registered TRouterDispatcher. Resulting IChainFlow will always have a Transient lifetime</param>
        /// <returns></returns>
        public static IServiceCollection AddBooleanRouterChainFlow<TRouterDispatcher>(
            this IServiceCollection services,
            ServiceLifetime dispatcherLifetime = ServiceLifetime.Transient) where TRouterDispatcher : class, IRouterDispatcher<bool>
        {
            RegisterWithLifetime<TRouterDispatcher>(services, dispatcherLifetime);
            services.AddSingleton(sp => new ChainFlowRegistration(
                typeof(TRouterDispatcher),
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
