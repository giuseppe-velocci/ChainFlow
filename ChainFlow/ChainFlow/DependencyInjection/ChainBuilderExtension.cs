using ChainFlow.ChainBuilder;
using ChainFlow.Documentables;
using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlow.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;

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

            services.AddSingleton(sp => new ChainFlowRegistration(ResolveChainFlowType<T>(), () => sp.GetRequiredService<T>()));

            return services;
        }

        private static Type ResolveChainFlowType<T>() where T : class, IChainFlow
        {
            Type flowType = typeof(T);

            if (typeof(IBooleanRouterFlow<>).IsAssignableFrom(flowType))
            {
                var genericArguments = flowType.GetGenericArguments();
                return typeof(IBooleanRouterFlow<>).MakeGenericType(genericArguments);
            }
            else
            {
                return flowType;
            }
        }
    }
}
