﻿using ChainFlow.ChainFlows.DataFlows;
using ChainFlow.DependencyInjection;
using ChainFlow.Interfaces.DataFlowsDependencies;
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
                .AddSingleton<IDataValidator<string>, StringValidator>()
                .AddChainFlow<DataValidatorFlow<string>>()

                .AddHostedService<ConsoleWorkflow>()
                ;
        })
        .UseConsoleLifetime(); // Use IConsoleLifetime to manage application lifetime;
    return host;
}

var host = CreateHostBuilder(args).Build();
try
{
    await host.RunAsync();
}
catch (OperationCanceledException _)
{
    await host.StopAsync(); 
}