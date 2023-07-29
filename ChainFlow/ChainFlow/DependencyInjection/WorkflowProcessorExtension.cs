using ChainFlow.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ChainFlow.DependencyInjection
{
    public static class WorkflowProcessorExtension
    {
        /// <summary>
        /// Add a workflow runner with the specified lifetime
        /// </summary>
        /// <typeparam name="TWorkflow">Type of the workflow runner. Must implement IWorkflow<TInput></typeparam>
        /// <typeparam name="TInput">Type of input, must match the one implemented with IWorkflow<TInput></typeparam>
        /// <param name="services">Current IServiceCollection</param>
        /// <param name="serviceLifetime">Lifetime for the registered IChainFlow</param>
        /// <returns>Current IServiceCollection</returns>
        public static IServiceCollection AddWorkflow<TWorkflow, TInput>(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TWorkflow : class, IWorkflow<TInput> 
            where TInput : notnull
        {
            if (serviceLifetime == ServiceLifetime.Transient)
            {
                services.AddTransient<IWorkflow<TInput>, TWorkflow>();
            }
            else if (serviceLifetime == ServiceLifetime.Scoped)
            {
                services.AddScoped<IWorkflow<TInput>, TWorkflow>();
            }
            else
            {
                services.AddSingleton<IWorkflow<TInput>, TWorkflow>();
            }
            return services;
        }
    }
}
