using ChainFlow.ChainFlows.DataFlows;
using ChainFlow.Interfaces;
using ChainFlow.Workflows;
using Console.Dispatchers;
using Console.Flows;
using Console.Validators;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ConsoleTests")]
namespace Console
{
    internal class ConsoleWorkflow : AbstractWorkflowRunner, IHostedService
    {
        public ConsoleWorkflow(IChainFlowBuilder chainBuilder) : base(chainBuilder)
        {
            Workflow = chainBuilder
                .With<DataValidatorFlow<string>>(nameof(StringValidator))
                .WithBooleanRouter<IsConsoleToTerminateDispatcher>(
                    (x) => x
                        .With<TerminateConsoleFlow>()
                        .Build(),
                    (x) => x
                        .With<DataValidatorFlow<string>>(nameof(NameValidator))
                        .With<GreeterFlow>()
                        .Build()
                )
                .Build();
        }

        protected override IChainFlow Workflow { get; set; }

        public override string Describe() => "A greeter console app with ChainFlow";

        public override string DescribeWorkflowEntryPoint() => "When user input is received";

        public override string GetWorkflowName() => nameof(ConsoleWorkflow);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                System.Console.WriteLine("Input your name or EXIT to quit.");
                var input = System.Console.ReadLine();
                var res = await Workflow.ProcessAsync(new ChainFlow.Models.RequestToProcess(input!), cancellationToken);
                System.Console.WriteLine($"{res.Message}{System.Environment.NewLine}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
