﻿using ChainFlow.ChainBuilder;
using ChainFlow.Documentables;
using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ChainFlow.DependencyInjection
{
    public static class HostExtensions
    {
        public static IHostBuilder InitializeWorkflowHostBuilder(this IHostBuilder hostBuilder, string[] args)
        {
            RunMode runMode = args.Contains("--doc") ?
                RunMode.Documentation :
                RunMode.Standard;

            if (runMode is RunMode.Documentation)
            {
                hostBuilder
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.TryAddSingleton<ISystemIoWriter, SystemIoWriter>();
                        services
                            .AddSingleton<DocumentableProgram>()
                            .AddSingleton<IChainFlowBuilder, DocumentChainFlowBuilder>();

                        RegisterAllIDocumentableWorkflows(services);
                    });
            }
            else
            {
                hostBuilder
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddSingleton<IChainFlowBuilder, ChainFlowBuilder>();
                    });
            }

            return new WorkflowHostBuilderDecorator(hostBuilder, runMode);
        }

        private static void RegisterAllIDocumentableWorkflows(IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var implementations = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IDocumentableWorkflow).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            implementations.ForEach(x =>
            {
                services.AddSingleton(typeof(IDocumentableWorkflow), x);
            });
        }
    }
}
