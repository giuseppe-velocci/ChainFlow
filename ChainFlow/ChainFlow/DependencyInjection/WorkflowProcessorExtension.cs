using ChainFlow.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ChainFlow.DependencyInjection
{
    public static class WorkflowProcessorExtension
    {
        public static IServiceCollection AddWorkflow<TProcessor, TInput>(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TProcessor : class, IWorkflow<TInput> 
            where TInput : notnull
        {
            if (serviceLifetime == ServiceLifetime.Transient)
            {
                services.AddTransient<IWorkflow<TInput>, TProcessor>();
            }
            else if (serviceLifetime == ServiceLifetime.Scoped)
            {
                services.AddScoped<IWorkflow<TInput>, TProcessor>();
            }
            else
            {
                services.AddSingleton<IWorkflow<TInput>, TProcessor>();
            }
            return services;
        }
    }
}
