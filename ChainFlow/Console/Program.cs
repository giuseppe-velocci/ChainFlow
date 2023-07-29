using ChainFlow.DependencyInjection;
using Console;
using Microsoft.Extensions.Hosting;

static IHostBuilder CreateHostBuilder(string[] args) 
{
    var host = Host
        .CreateDefaultBuilder(args)
        .InitializeWorkflowHostBuilder(args) // this is needed to initialize ChainFlow
        .ConfigureServices((hostContext, services) =>
        {
            services
                .AddWorkflow<ConsoleWorkflow, string>()

                ;
        });
    return host;
}

var host = CreateHostBuilder(args).Build();
await host.RunAsync();
