using ChainFlow.ChainBuilder;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ChainFlow.DependencyInjection
{
    public static class ChainBuilderExtension
    {
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

            services.AddSingleton(sp => new ChainFlowRegistration(typeof(T), () => sp.GetRequiredService<T>()));

            return services;
        }

        public static IServiceCollection AddChainFlowBuilder(this IServiceCollection services)
        {
            services.AddSingleton<IChainFlowBuilder, ChainFlowBuilder>();
            return services;
        }
    }
}
