using ChainFlow.ChainFlows.DataFlows;
using ChainFlow.Interfaces;
using ChainFlow.Workflows;
using Console.Dispatchers;
using Console.Flows;
using Microsoft.Extensions.Hosting;

namespace Console
{
    internal class ConsoleWorkflow : AbstractWorkflowRunner, IHostedService
    {
        public ConsoleWorkflow(IChainFlowBuilder chainBuilder) : base(chainBuilder)
        {
            Workflow = chainBuilder
                .WithBooleanRouter<TerminateConsoleDispatcher>(
                    (x) => x
                        .With<TerminateConsoleFlow>()
                        .Build(),
                    (x) => x
                        .With<DataValidatorFlow<string>>()
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
                var res = await Workflow.ProcessAsync(new ChainFlow.Models.ProcessingRequest(input!), cancellationToken);
                System.Console.WriteLine($"{res.Message}{System.Environment.NewLine}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
