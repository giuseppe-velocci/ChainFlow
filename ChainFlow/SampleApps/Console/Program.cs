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
        .InitializeWorkflowHostBuilder(args) // this is needed to initialize ChainFlow framework
        .ConfigureServices((hostContext, services) =>
        {
            services
                // register dependencies    
                .AddSingleton<StringValidator>()
                .AddSingleton<NameValidator>()

                // register ChainFlows
                .AddChainFlow<GreeterFlow>()
                .AddChainFlow<TerminateConsoleFlow>()
                .AddBooleanRouterChainFlow<IsConsoleToTerminateDispatcher>()
                // register 2 concrete instances of DataValidatorFlow<string> with a tag to let DI identify them
                .AddChainFlow((sp) => new DataValidatorFlow<string>(sp.GetRequiredService<StringValidator>()), nameof(StringValidator))
                .AddChainFlow((sp) => new DataValidatorFlow<string>(sp.GetRequiredService<NameValidator>()), nameof(NameValidator))
                
                // register console main hosted service
                .AddHostedService<ConsoleWorkflow>();
        })
        .UseConsoleLifetime(); // Use IConsoleLifetime to manage application lifetime;
    return host;
}

var host = CreateHostBuilder(args).Build();
try
{
    await host.RunAsync();
}
catch (OperationCanceledException)
{
    await host.StopAsync();
}