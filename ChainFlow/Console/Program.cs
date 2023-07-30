﻿using ChainFlow.DependencyInjection;
using Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

static IHostBuilder CreateHostBuilder(string[] args) 
{
    var host = Host
        .CreateDefaultBuilder(args)
        .InitializeWorkflowHostBuilder(args) // this is needed to initialize ChainFlow
        .ConfigureServices((hostContext, services) =>
        {
            services
                .AddSingleton<ConsoleWorkflow>()
                .AddChainFlow<ValidateInputFlow>()
                ;
        });
    return host;
}

var host = CreateHostBuilder(args).Build();
await host.RunAsync();
