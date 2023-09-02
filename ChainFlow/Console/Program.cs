using ChainFlow.ChainFlows.DataFlows;
using ChainFlow.DependencyInjection;
using Console;
using Console.Dispatchers;
using Console.Flows;
using Console.Validators;
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
                .AddSingleton<StringValidator>()
                .AddSingleton<NameValidator>()
                .AddChainFlow<TerminateConsoleFlow>()
                .AddBooleanRouterChainFlow<IsConsoleToTerminateDispatcher>()
                .AddChainFlow((sp) => new DataValidatorFlow<string>(sp.GetRequiredService<StringValidator>()), nameof(StringValidator))
                .AddChainFlow((sp) => new DataValidatorFlow<string>(sp.GetRequiredService<NameValidator>()), nameof(NameValidator))
                .AddChainFlow<GreeterFlow>()

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